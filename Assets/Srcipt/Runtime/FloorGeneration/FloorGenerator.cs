using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator
{
    // --- Réglages ---
    private const float RoomDensity = 0.55f;             // part de la grille occupée par des salles
    private const float MaxHighDegreeRatio = 0.2f;       // max 20% de salles avec plus de 2 voisins
    private const float SingleNeighborPreference = 0.85f; // 85% du temps, on prolonge un couloir plutôt que de refermer une boucle
    private const int MaxAttempts = 30;                  // essais si la génération se retrouve coincée

    private static readonly Direction[] AllDirections =
        { Direction.North, Direction.South, Direction.East, Direction.West };

    private System.Random rng;

    public FloorGenerator(int seed)
    {
        rng = new System.Random(seed);
    }

    public FloorGrid Generate(int floorLevel)
    {
        int gridSize = 4 + (floorLevel - 1); // étage 1 = 4x4, étage 2 = 5x5...
        int targetRoomCount = Mathf.RoundToInt(gridSize * gridSize * RoomDensity);

        FloorGrid best = null;
        int bestCount = -1;

        for (int attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var grid = new FloorGrid(gridSize, gridSize);
            int count = PlaceRooms(grid, targetRoomCount);

            if (count > bestCount)
            {
                best = grid;
                bestCount = count;
            }

            if (count >= targetRoomCount) break;
        }

        BuildConnections(best);
        AssignRoomTypes(best);

        return best;
    }

    // ---------------------------------------------------------------
    // 1. Placement des salles
    // ---------------------------------------------------------------

    private int PlaceRooms(FloorGrid grid, int targetRoomCount)
    {
        int maxHighDegree = Mathf.FloorToInt(targetRoomCount * MaxHighDegreeRatio);
        int highDegreeCount = 0;

        int cx = grid.Width / 2;
        int cy = grid.Height / 2;
        grid.Rooms[cx, cy] = new RoomNode(cx, cy, RoomType.Start);
        int roomCount = 1;

        while (roomCount < targetRoomCount)
        {
            var candidates = GetCandidates(grid, maxHighDegree - highDegreeCount);
            if (candidates.Count == 0) break; // coincé, on s'arrête là

            // on préfère les cases qui prolongent un couloir (1 seul voisin)
            var singles = candidates.FindAll(c => c.neighbors == 1);
            var pool = (singles.Count > 0 && rng.NextDouble() < SingleNeighborPreference)
                ? singles
                : candidates;

            var chosen = pool[rng.Next(pool.Count)];

            grid.Rooms[chosen.x, chosen.y] = new RoomNode(chosen.x, chosen.y, RoomType.Neutral);
            highDegreeCount += chosen.newHighDegree;
            roomCount++;
        }

        return roomCount;
    }

    // Liste toutes les cases où on a le droit de poser une salle à cet instant
    private List<(int x, int y, int neighbors, int newHighDegree)> GetCandidates(FloorGrid grid, int remainingBudget)
    {
        var result = new List<(int x, int y, int neighbors, int newHighDegree)>();

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                if (grid.Rooms[x, y] != null) continue;

                int neighbors = CountOrthogonalNeighbors(grid, x, y);
                if (neighbors == 0) continue;                 // doit être collée à une salle existante
                if (CreatesFullBlock(grid, x, y)) continue;   // règle des 3 salles sur 4 cases

                int newHighDegree = CountNewHighDegree(grid, x, y);
                if (newHighDegree > remainingBudget) continue; // règle des 20%

                result.Add((x, y, neighbors, newHighDegree));
            }
        }

        return result;
    }

    private int CountOrthogonalNeighbors(FloorGrid grid, int x, int y)
    {
        int count = 0;

        foreach (var dir in AllDirections)
        {
            var o = dir.ToOffset();
            if (grid.GetRoom(x + o.x, y + o.y) != null) count++;
        }

        return count;
    }

    // Poser une salle en (x, y) complèterait-elle un carré de 2x2 entièrement occupé ?
    private bool CreatesFullBlock(FloorGrid grid, int x, int y)
    {
        // les 4 carrés 2x2 qui contiennent la case (x, y), repérés par leur coin bas-gauche
        for (int bx = x - 1; bx <= x; bx++)
        {
            for (int by = y - 1; by <= y; by++)
            {
                int occupied = 0;

                for (int cx = bx; cx <= bx + 1; cx++)
                {
                    for (int cy = by; cy <= by + 1; cy++)
                    {
                        if (cx == x && cy == y) continue; // la case candidate elle-même
                        if (grid.GetRoom(cx, cy) != null) occupied++;
                    }
                }

                if (occupied == 3) return true; // 3 déjà posées + la nouvelle = 4
            }
        }

        return false;
    }

    // Combien de salles deviendraient des "carrefours" (3+ voisins) si on posait ici ?
    private int CountNewHighDegree(FloorGrid grid, int x, int y)
    {
        int newHigh = 0;
        int selfNeighbors = 0;

        foreach (var dir in AllDirections)
        {
            var o = dir.ToOffset();
            var neighbor = grid.GetRoom(x + o.x, y + o.y);
            if (neighbor == null) continue;

            selfNeighbors++;

            // ce voisin a déjà 2 voisins : avec la nouvelle salle, il passe à 3
            if (CountOrthogonalNeighbors(grid, neighbor.X, neighbor.Y) == 2) newHigh++;
        }

        if (selfNeighbors > 2) newHigh++; // la nouvelle salle est elle-même un carrefour

        return newHigh;
    }

    // ---------------------------------------------------------------
    // 2. Portes entre salles voisines
    // ---------------------------------------------------------------

    private void BuildConnections(FloorGrid grid)
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var room = grid.Rooms[x, y];
                if (room == null) continue;

                foreach (var dir in AllDirections)
                {
                    var o = dir.ToOffset();
                    if (grid.GetRoom(x + o.x, y + o.y) != null)
                        room.Connections.Add(dir);
                }
            }
        }
    }

    // ---------------------------------------------------------------
    // 3. Types de salles
    // ---------------------------------------------------------------

    private void AssignRoomTypes(FloorGrid grid)
    {
        var rooms = new List<RoomNode>();
        RoomNode startRoom = null;

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var room = grid.Rooms[x, y];
                if (room == null) continue;

                rooms.Add(room);
                if (room.Type == RoomType.Start) startRoom = room;
            }
        }

        var distances = ComputeDistances(grid, startRoom);

        // Boss : cul-de-sac (1 connexion) le plus loin du Start
        RoomNode bossRoom = null;
        int maxDistance = -1;

        foreach (var room in rooms)
        {
            if (room == startRoom || room.Connections.Count != 1) continue;

            if (distances[room] > maxDistance)
            {
                maxDistance = distances[room];
                bossRoom = room;
            }
        }

        // secours : si aucun cul-de-sac, la salle la plus loin
        if (bossRoom == null)
        {
            foreach (var room in rooms)
            {
                if (room == startRoom) continue;

                if (distances[room] > maxDistance)
                {
                    maxDistance = distances[room];
                    bossRoom = room;
                }
            }
        }

        if (bossRoom != null) bossRoom.Type = RoomType.Boss;

        foreach (var room in rooms)
        {
            if (room == startRoom || room == bossRoom) continue;
            room.Type = RollRandomRoomType();
        }
    }

    private Dictionary<RoomNode, int> ComputeDistances(FloorGrid grid, RoomNode startRoom)
    {
        var distances = new Dictionary<RoomNode, int> { { startRoom, 0 } };
        var queue = new Queue<RoomNode>();
        queue.Enqueue(startRoom);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var dir in current.Connections)
            {
                var o = dir.ToOffset();
                var neighbor = grid.GetRoom(current.X + o.x, current.Y + o.y);

                if (neighbor == null || distances.ContainsKey(neighbor)) continue;

                distances[neighbor] = distances[current] + 1;
                queue.Enqueue(neighbor);
            }
        }

        return distances;
    }

    private RoomType RollRandomRoomType()
    {
        int roll = rng.Next(0, 100);

        if (roll < 20) return RoomType.Treasure;
        if (roll < 55) return RoomType.Monster;
        if (roll < 70) return RoomType.Heal;
        return RoomType.Neutral;
    }
}