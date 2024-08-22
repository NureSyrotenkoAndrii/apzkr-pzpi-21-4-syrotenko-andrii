using Microsoft.EntityFrameworkCore;
using SafeEscape.Interfaces;
using SafeEscape.Models;
using SafeEscape.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class RoomEdgeService : IRoomEdgeService
    {
        private readonly ApplicationDbContext _context;

        public RoomEdgeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all RoomEdge records from the database.
        public async Task<IEnumerable<RoomEdge>> GetAllAsync()
        {
            return await _context.RoomEdges.ToListAsync();
        }

        // Retrieves a specific RoomEdge record by its ID.
        public async Task<RoomEdge> GetByIdAsync(Guid id)
        {
            return await _context.RoomEdges.FindAsync(id);
        }

        // Creates a new RoomEdge record after validating that the related rooms exist.
        public async Task CreateAsync(RoomEdge roomEdge)
        {
            if (!await _context.Rooms.AnyAsync(r => r.Id == roomEdge.Room1Id))
            {
                throw new KeyNotFoundException("Room1 not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == roomEdge.Room2Id))
            {
                throw new KeyNotFoundException("Room2 not found");
            }

            await _context.RoomEdges.AddAsync(roomEdge);
            await _context.SaveChangesAsync();
        }

        // Updates an existing RoomEdge record after validating the related rooms exist.
        public async Task UpdateAsync(Guid id, RoomEdge roomEdge)
        {
            var existingRoomEdge = await _context.RoomEdges.FindAsync(id);
            if (existingRoomEdge == null)
            {
                throw new KeyNotFoundException("RoomEdge not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == roomEdge.Room1Id))
            {
                throw new KeyNotFoundException("Room1 not found");
            }

            if (!await _context.Rooms.AnyAsync(r => r.Id == roomEdge.Room2Id))
            {
                throw new KeyNotFoundException("Room2 not found");
            }

            existingRoomEdge.Room1Id = roomEdge.Room1Id;
            existingRoomEdge.Room2Id = roomEdge.Room2Id;
            existingRoomEdge.Distance = roomEdge.Distance;

            _context.RoomEdges.Update(existingRoomEdge);
            await _context.SaveChangesAsync();
        }

        // Deletes a RoomEdge record by its ID.
        public async Task DeleteAsync(Guid id)
        {
            var roomEdge = await _context.RoomEdges.FindAsync(id);
            if (roomEdge == null)
            {
                throw new KeyNotFoundException("RoomEdge not found");
            }

            _context.RoomEdges.Remove(roomEdge);
            await _context.SaveChangesAsync();
        }

        // Calculates the evacuation route from a start room to the nearest exit using Dijkstra's algorithm.
        public async Task<EvacuationRouteResponseDto> GetEvacuationRouteAsync(Guid startRoomId)
        {
            var rooms = await _context.Rooms.Include(r => r.RoomEdges1).Include(r => r.RoomEdges2).ToListAsync();
            var roomEdges = await _context.RoomEdges.ToListAsync();
            var exits = await _context.Exits.ToListAsync();
            var stairs = await _context.Stairs.ToListAsync();

            var graph = new Dictionary<Guid, Dictionary<Guid, int>>();
            var roomNames = rooms.ToDictionary(r => r.Id, r => r.Name);

            // Build the graph with room edges.
            foreach (var edge in roomEdges)
            {
                if (!graph.ContainsKey(edge.Room1Id))
                {
                    graph[edge.Room1Id] = new Dictionary<Guid, int>();
                }
                if (!graph.ContainsKey(edge.Room2Id))
                {
                    graph[edge.Room2Id] = new Dictionary<Guid, int>();
                }
                graph[edge.Room1Id][edge.Room2Id] = edge.Distance;
                graph[edge.Room2Id][edge.Room1Id] = edge.Distance;
            }

            // Add stair connections to the graph.
            foreach (var stair in stairs)
            {
                if (!graph.ContainsKey(stair.Floor1RoomId))
                {
                    graph[stair.Floor1RoomId] = new Dictionary<Guid, int>();
                }
                if (!graph.ContainsKey(stair.Floor2RoomId))
                {
                    graph[stair.Floor2RoomId] = new Dictionary<Guid, int>();
                }
                graph[stair.Floor1RoomId][stair.Floor2RoomId] = 1;
                graph[stair.Floor2RoomId][stair.Floor1RoomId] = 1;
            }

            return DijkstraAlgorithm(graph, startRoomId, exits.Select(e => e.RoomId).ToList(), roomNames);
        }

        // Dijkstra's algorithm to find the shortest path in the graph.
        private EvacuationRouteResponseDto DijkstraAlgorithm(Dictionary<Guid, Dictionary<Guid, int>> graph, Guid startRoomId, List<Guid> exitRoomIds, Dictionary<Guid, string> roomNames)
        {
            var distances = new Dictionary<Guid, int>();
            var previous = new Dictionary<Guid, Guid?>();
            var nodes = new List<Guid>();

            // Initialize distances and previous node mappings.
            foreach (var node in graph)
            {
                distances[node.Key] = node.Key == startRoomId ? 0 : int.MaxValue;
                previous[node.Key] = null;
                nodes.Add(node.Key);
            }

            // Dijkstra's algorithm main loop.
            while (nodes.Count != 0)
            {
                nodes.Sort((x, y) => distances[x] - distances[y]);
                var smallest = nodes[0];
                nodes.Remove(smallest);

                if (distances[smallest] == int.MaxValue)
                {
                    break;
                }

                if (exitRoomIds.Contains(smallest))
                {
                    var path = new List<Guid>();
                    while (previous[smallest] != null)
                    {
                        path.Add(smallest);
                        smallest = previous[smallest].Value;
                    }

                    path.Add(startRoomId);
                    path.Reverse();

                    var roomNamesPath = path.Select(roomId => roomNames[roomId]).ToList();

                    return new EvacuationRouteResponseDto
                    {
                        RoomIds = path,
                        RoomNames = roomNamesPath,
                        TotalDistance = distances[path.Last()]
                    };
                }

                foreach (var neighbor in graph[smallest])
                {
                    var alt = distances[smallest] + neighbor.Value;
                    if (alt < distances[neighbor.Key])
                    {
                        distances[neighbor.Key] = alt;
                        previous[neighbor.Key] = smallest;
                    }
                }
            }

            return null;
        }
    }
}
