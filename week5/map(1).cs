using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map_connection
{
	public enum types { neighbor, jump, teleport }
	public types type = types.neighbor;
	public int id = -1;

	public map_connection clone()
	{
		map_connection connection = new map_connection();
		connection.type = type;
		connection.id = id;
		return connection;
	}

}

public class map_tile
{

	public int id = -1;
	public int blocking = 0;
	public board.sides goal = board.sides.none;
	public Dictionary<int, map_connection> connections = new Dictionary<int, map_connection>();

	public void connect(int connect_tile_id, map_connection.types type)
	{
		if (connect_tile_id == id) return;
		if (connections.ContainsKey(connect_tile_id)) return;
		map_connection connection = new map_connection();
		connection.id = connect_tile_id;
		connection.type = type;
		connections.Add(connect_tile_id, connection);
	}

	public int get_teleport()
	{
		foreach (int tile_id in connections.Keys)
		{
			if (connections[tile_id].type != map_connection.types.teleport) continue;
			return connections[tile_id].id;
		}
		return -1;
	}

	public map_tile clone()
	{
		map_tile tile = new map_tile();
		tile.id = id;
		tile.goal = goal;
		foreach (KeyValuePair<int, map_connection> pair in connections) tile.connections.Add(pair.Key, pair.Value.clone());
		return tile;
	}

}

public class map_unit
{
	public int id = -1;
	public unit.modes mode = unit.modes.basic;
	public unit.types type = unit.types.none;
	public board.sides side = board.sides.none;
	public int prev_tile_turns = 0;
	public int prev_tile = -1;
	public int phased_turns = 0;
	public int freeze_turns = 0;
	public int defence_turns = 0;
	public unit.types hidden_setto = unit.types.none;

	public map_unit clone()
	{
		map_unit unit = new map_unit();
		unit.id = id;
		unit.mode = mode;
		unit.type = type;
		unit.side = side;
		unit.prev_tile_turns = prev_tile_turns;
		unit.prev_tile = prev_tile;
		unit.phased_turns = phased_turns;
		unit.freeze_turns = freeze_turns;
		unit.defence_turns = defence_turns;
		unit.hidden_setto = hidden_setto;
		return unit;
	}

	public void round_pass(board.sides current)
	{

		if (side == current && phased_turns > 0) phased_turns--;
		if (side == current && defence_turns > 0) defence_turns--;
		if (side != current && freeze_turns > 0) freeze_turns--;

		if (side != current && prev_tile > -1)
		{
			prev_tile_turns--;
			if (prev_tile_turns < 1) prev_tile = -1;
		}

	}

}

public class map_move
{
	public enum types { none, nomove, move, destroy, attack_lose, attack_win, attack_draw };
	public types type = types.none;
	public int origin_tile_id = -1;

	public int target_tile_id = -1;
	public int target_tile_damage = 0;

	public int taregt_tile_real_id = -1; // because target_tile_id can be a teleport tile

	public unit.types origin_tile_reveal = unit.types.none;
	public unit.types target_tile_reveal = unit.types.none;

	public int score = -999999999;
	public int outcome = 0;
	
}

public class map
{

	public game game;
	public int max_distance = 0;
	public Dictionary<int, map_tile> tiles = new Dictionary<int, map_tile>();
	public Dictionary<int, map_unit> units = new Dictionary<int, map_unit>();

	public void load(game game)
	{
		this.game = game;
		load_tiles();
		load_units();
		//max_distance = tiles.Count * tiles.Count;
		max_distance = get_max_distance();

	}

	public void load_tiles()
	{

		tiles = new Dictionary<int, map_tile>();

		foreach (hex hex in game.grid.Values)
		{

			map_tile map_tile = new map_tile();
			map_tile.id = hex.tile_id;
			if (hex.type == hex.types.goal) map_tile.goal = hex.side;

			List<hex> neighbors = hex.GetConnections(true);
			foreach (hex tile in neighbors)
			{
				map_tile.connect(tile.tile_id, map_connection.types.neighbor);
			}

			if (hex.ability == hex.abilities.jump)
			{
				List<hex> skips = game.GetTilesAtExactRange(hex, 2);
				foreach (hex tile in skips)
				{
					if (!tile) continue;
					if (!tile.isset) continue;
					map_tile.connect(tile.tile_id, map_connection.types.jump);
				}
			}

			hex teleport = game.get(hex.teleport_id);
			if (teleport) map_tile.connect(teleport.tile_id, map_connection.types.teleport);

			if (hex.type == hex.types.blocking) map_tile.blocking = hex.hp;

			tiles.Add(map_tile.id, map_tile);

		}

	}

	public void load_units()
	{

		units = new Dictionary<int, map_unit>();

		foreach (hex hex in game.grid.Values)
		{
			if (!hex.unit) continue;
			map_unit unit = new map_unit();
			unit.id = hex.unit.id;
			unit.mode = hex.unit.mode;
			unit.type = hex.unit.type;
			unit.side = hex.unit.side;
			unit.prev_tile_turns = hex.unit.prev_tile_turns;
			unit.prev_tile = hex.unit.prev_tile ? hex.unit.prev_tile.tile_id : -1;
			unit.phased_turns = hex.unit.phased_turns;
			unit.freeze_turns = hex.unit.freeze_turns;
			unit.defence_turns = hex.unit.defence_turns;
			unit.hidden_setto = hex.unit.hidden_setto;
			units.Add(hex.tile_id, unit);
		}
	}

	public map clone()
	{
		map map = new map();
		map.game = game;
		map.max_distance = max_distance;

		map.tiles = new Dictionary<int, map_tile>();
		foreach (KeyValuePair<int, map_tile> pair in tiles) map.tiles.Add(pair.Key, pair.Value.clone());

		map.units = new Dictionary<int, map_unit>();
		foreach (KeyValuePair<int, map_unit> pair in units) map.units.Add(pair.Key, pair.Value.clone());

		return map;
	}

	public void round_pass(board.sides side)
	{
		foreach (int tile_id in units.Keys) units[tile_id].round_pass(side);
	}

	public List<map_move> get_moves(board.sides side)
	{

		List<map_move> moves = new List<map_move>();
		map_move move = null;
		map_unit other = null;
		int teleport_id = -1;

		// add units moves
		foreach (int tile_id in units.Keys)
		{
			map_unit unit = units[tile_id];
			if (unit.side != side) continue;
			if (unit.freeze_turns > 0) continue;

			foreach (int connect_tile_id in tiles[tile_id].connections.Keys)
			{
				map_connection connection = tiles[tile_id].connections[connect_tile_id];
				switch (connection.type)
				{
					case map_connection.types.neighbor:

						if (tiles[connection.id].blocking > 0) // blocking attack
						{
							move = new map_move();
							move.type = map_move.types.destroy;
							move.origin_tile_id = tile_id;
							move.target_tile_id = connection.id;
							move.taregt_tile_real_id = connection.id;
							move.target_tile_damage = 1;
							moves.Add(move);
							continue;
						}

						if (units.ContainsKey(connection.id)) // unit attack 
						{
							other = units[connection.id];
							if (other.side == side) continue;
							if (other.defence_turns > 0)
							{
								move = new map_move();
								move.type = map_move.types.nomove;
								moves.Add(move);
								continue;
							}

						   
							if (unit.type == ginra.unit.types.hidden && other.type == ginra.unit.types.hidden)
							{
								// this scenario is too random
								move = new map_move();
								move.type = map_move.types.attack_draw;
								move.origin_tile_id = tile_id;
								move.target_tile_id = connection.id;
								move.taregt_tile_real_id = connection.id;
								move.origin_tile_reveal = ginra.unit.types.rock;
								move.target_tile_reveal = ginra.unit.types.rock;
								moves.Add(move);
							}

							
							if (unit.type == ginra.unit.types.hidden)
							{
								move = new map_move();
								move.type = map_move.types.attack_win;
								move.origin_tile_id = tile_id;
								move.target_tile_id = connection.id;
								move.taregt_tile_real_id = connection.id;
								if (other.type == ginra.unit.types.rock) move.origin_tile_reveal = ginra.unit.types.paper;
								if (other.type == ginra.unit.types.paper) move.origin_tile_reveal = ginra.unit.types.scissors;
								if (other.type == ginra.unit.types.scissors) move.origin_tile_reveal = ginra.unit.types.rock;
								moves.Add(move);
								continue;
							}

							
							if (other.type == ginra.unit.types.hidden)
							{
								move = new map_move();
								move.type = map_move.types.attack_lose;
								move.origin_tile_id = tile_id;
								move.target_tile_id = connection.id;
								move.taregt_tile_real_id = connection.id;
								if (unit.type == ginra.unit.types.rock) move.target_tile_reveal = ginra.unit.types.paper;
								if (unit.type == ginra.unit.types.paper) move.target_tile_reveal = ginra.unit.types.scissors;
								if (unit.type == ginra.unit.types.scissors) move.target_tile_reveal = ginra.unit.types.rock;
								moves.Add(move);
								continue;
							}

							if (unit.type == other.type) continue; // cant do draw attacks

							// win
							if (
								 (unit.type == ginra.unit.types.rock && other.type == ginra.unit.types.scissors) ||
								 (unit.type == ginra.unit.types.paper && other.type == ginra.unit.types.rock) ||
								 (unit.type == ginra.unit.types.scissors && other.type == ginra.unit.types.paper)
								 )
							{
								move = new map_move();
								move.type = map_move.types.attack_win;
								move.origin_tile_id = tile_id;
								move.target_tile_id = connection.id;
								move.taregt_tile_real_id = connection.id;
								moves.Add(move);
								continue;
							}

							// lose
							if (
								 (other.type == ginra.unit.types.rock && unit.type == ginra.unit.types.scissors) ||
								 (other.type == ginra.unit.types.paper && unit.type == ginra.unit.types.rock) ||
								 (other.type == ginra.unit.types.scissors && unit.type == ginra.unit.types.paper)
								 )
							{
								move = new map_move();
								move.type = map_move.types.attack_lose;
								move.origin_tile_id = tile_id;
								move.target_tile_id = connection.id;
								move.taregt_tile_real_id = connection.id;
								moves.Add(move);
								continue;
							}

							continue;
						}

						
						if (unit.prev_tile == connection.id) continue;

						// teleport
						teleport_id = tiles[connection.id].get_teleport();
						if (teleport_id >= 0 && !units.ContainsKey(teleport_id))
						{
							move = new map_move();
							move.type = map_move.types.move;
							move.origin_tile_id = tile_id;
							move.target_tile_id = teleport_id;
							move.taregt_tile_real_id = connection.id;
							moves.Add(move);
							continue;
						}

						// move
						move = new map_move();
						move.type = map_move.types.move;
						move.origin_tile_id = tile_id;
						move.target_tile_id = connection.id;
						move.taregt_tile_real_id = connection.id;
						moves.Add(move);
						continue;

					case map_connection.types.jump:
						if (tiles[connection.id].blocking > 0) continue; // can't jump on blocking
						if (units.ContainsKey(connection.id)) continue; // can't jump on unit
						if (unit.prev_tile == connection.id) continue;

						// teleport
						teleport_id = tiles[connection.id].get_teleport();
						if (teleport_id >= 0 && !units.ContainsKey(teleport_id))
						{
							move = new map_move();
							move.type = map_move.types.move;
							move.origin_tile_id = tile_id;
							move.target_tile_id = teleport_id;
							move.taregt_tile_real_id = connection.id;
							moves.Add(move);
							continue;
						}

						// move
						move = new map_move();
						move.type = map_move.types.move;
						move.origin_tile_id = tile_id;
						move.target_tile_id = connection.id;
						move.taregt_tile_real_id = connection.id;
						moves.Add(move);
						continue;

					case map_connection.types.teleport:
						// do nothing, cant teleport while standing on the teleport tile
						continue;

				}

			}


		}

		return moves;

	}

	public map apply_move(board.sides side, map_move move)
	{
		map map = clone();

		switch (move.type)
		{

			case map_move.types.none:
				// do nothing
				break;
			case map_move.types.nomove:
				// do nothing
				break;
			case map_move.types.move:
				map.move_unit(move.origin_tile_id, move.target_tile_id);
				break;
			case map_move.types.destroy:
				map.tiles[move.target_tile_id].blocking -= move.target_tile_damage;
				break;
			case map_move.types.attack_lose:
				if (move.target_tile_reveal == unit.types.rock) map.units[move.target_tile_id].type = unit.types.rock;
				if (move.target_tile_reveal == unit.types.paper) map.units[move.target_tile_id].type = unit.types.paper;
				if (move.target_tile_reveal == unit.types.scissors) map.units[move.target_tile_id].type = unit.types.scissors;
				map.units.Remove(move.origin_tile_id);
				break;
			case map_move.types.attack_win:
				if (move.origin_tile_reveal == unit.types.rock) map.units[move.origin_tile_id].type = unit.types.rock;
				if (move.origin_tile_reveal == unit.types.paper) map.units[move.origin_tile_id].type = unit.types.paper;
				if (move.origin_tile_reveal == unit.types.scissors) map.units[move.origin_tile_id].type = unit.types.scissors;
				map.units.Remove(move.target_tile_id);
				map.move_unit(move.origin_tile_id, move.target_tile_id);
				break;
			case map_move.types.attack_draw:
				if (move.origin_tile_reveal == unit.types.rock) map.units[move.origin_tile_id].type = unit.types.rock;
				if (move.origin_tile_reveal == unit.types.paper) map.units[move.origin_tile_id].type = unit.types.paper;
				if (move.origin_tile_reveal == unit.types.scissors) map.units[move.origin_tile_id].type = unit.types.scissors;
				if (move.target_tile_reveal == unit.types.rock) map.units[move.target_tile_id].type = unit.types.rock;
				if (move.target_tile_reveal == unit.types.paper) map.units[move.target_tile_id].type = unit.types.paper;
				if (move.target_tile_reveal == unit.types.scissors) map.units[move.target_tile_id].type = unit.types.scissors;
				break;

		}

		map.round_pass(side);
		return map;
	}

	public void remove_unit(int from_tile_id)
	{
		if (!units.ContainsKey(from_tile_id)) return;
		units.Remove(from_tile_id);
	}

	public void move_unit(int from_tile_id, int to_tile_id)
	{
		if (from_tile_id == to_tile_id) return;
		if (!units.ContainsKey(from_tile_id)) return;
		if (units.ContainsKey(to_tile_id)) return;
		map_unit unit = units[from_tile_id];
		units.Remove(from_tile_id);
		units.Add(to_tile_id, unit);
		unit.prev_tile = from_tile_id;
		unit.prev_tile_turns = 1 * 2;
	}

	public int distance(int origin, int goal)
	{

		int max_steps = tiles.Count * tiles.Count;
		//int max_steps = max_distance;

		Dictionary<int, flow> result = new Dictionary<int, flow>();
		List<flow> opened = new List<flow>() { new flow(origin, 0, 0, 0) };
		List<flow> conn = new List<flow>();

		for (int step = 0; step <= max_steps; step++)
		{

			conn.Clear();
			foreach (flow flow in opened)
			{
				if (result.ContainsKey(flow.id)) continue;
				if (flow.steps > 0)
				{
					flow.steps--;
					conn.Add(flow);
					continue;
				}
				if (flow.id != origin) result.Add(flow.id, new flow(flow.id, step, flow.connect, flow.cost));
				foreach (int connection_id in tiles[flow.id].connections.Keys)
					conn.Add(new flow(connection_id, tiles[connection_id].blocking, flow.id, flow.cost));
			}

			opened.Clear();
			foreach (flow flow in conn)
			{
				if (result.ContainsKey(flow.id)) continue;
				opened.Add(flow);
			}

			if (opened.Count < 1) break;

		}

		conn.Clear();
		opened.Clear();

		if (!result.ContainsKey(goal)) return max_steps;

		return result[goal].steps;

	}

	public int outcome(board.sides side)
	{
		// 1=win , -1 lose

		board.sides other = side == board.sides.red ? board.sides.blue : board.sides.red;

		int side_units = 0;
		int other_units = 0;

		foreach (int tile_id in units.Keys)
		{
			if (tiles[tile_id].goal == side) return 1;
			if (tiles[tile_id].goal == other) return -1;
			if (units[tile_id].side == side) side_units++;
			if (units[tile_id].side == other) other_units++;
		}

		if (side_units < 1) return -1;
		if (other_units < 1) return 1;

		return 0;
	}

	public int get_max_distance()
	{
		int max = 0;
		foreach (int i in tiles.Keys)
		{
			foreach (int j in tiles.Keys)
			{
				if (i == j) continue;
				int dis = distance(i, j);
				if (dis > max) max = dis;
			}
		}
		return max;
	}

	public int evaluate(board.sides side)
	{

		int side_min_distance_from_goal = max_distance;
		int other_min_distance_from_goal = max_distance;
		int sum_distance_to_win = 0;
		int sum_distance_to_lose = 0;
		int side_total_units = 0;
		int other_total_units = 0;

		foreach (map_tile tile in tiles.Values)
		{
			if (tile.goal == board.sides.none) continue;
			foreach (int tile_id in units.Keys)
			{
				if (units[tile_id].side != tile.goal) continue;
				int dist = distance(tile_id, tile.id);
				if (side == tile.goal)
				{
					if (dist <= 1) return 100000;
					side_min_distance_from_goal = Mathf.Min(side_min_distance_from_goal, dist);
				}
				else
				{
					if (dist <= 1) return -100000;
					other_min_distance_from_goal = Mathf.Min(other_min_distance_from_goal, dist);
				}

			}
		}


		foreach (int a in units.Keys)
		{
			if (units[a].side == side) side_total_units += 1;
			else other_total_units += 1;

			if (units[a].side != side) continue;
			


			foreach (int b in units.Keys)
			{
				if (units[a].side == units[b].side) continue;
				if (units[a].type == units[b].type) continue; // draw
				bool win = (
					   (units[a].type == unit.types.rock && units[b].type == unit.types.scissors) ||
					   (units[a].type == unit.types.paper && units[b].type == unit.types.rock) ||
					   (units[a].type == unit.types.scissors && units[b].type == unit.types.paper) ||
					   units[a].type == unit.types.hidden
					   );
				int dist = distance(a, b);

				if (win) sum_distance_to_win += (max_distance - dist) * 10;
				else sum_distance_to_lose += (max_distance - dist) * 8;


			}
		}


		int score = 0;

		score += side_total_units * 2000;
		score += other_total_units * -1000;

		score += 500 * (max_distance - side_min_distance_from_goal);
		if (other_min_distance_from_goal > 5) score -= 500 * (max_distance * 4 - side_min_distance_from_goal);
		else if (other_min_distance_from_goal > 2) score -= 500 * (max_distance * 8 - side_min_distance_from_goal);
		else score -= 500 * (max_distance * 16 - side_min_distance_from_goal);

		score += 500 * sum_distance_to_win;
		score -= 500 * sum_distance_to_lose;

		score = sum_distance_to_win;

		return score;
	}

	public int predict(board.sides side, board.sides round_side, map movemap, int alpha, int beta, int max_depth = 2)
	{
		int value = 0;

		if (max_depth <= 0)
		{
			value = movemap.evaluate(side);
			return value;
		}

		if (side != round_side)
		{

			value = 1000000;
			List<map_move> moves = movemap.get_moves(round_side);
			foreach (ai.map_move move in moves)
			{
				map movemap1 = apply_move(round_side, move);
				int res = movemap1.outcome(side) * 100000;

				if (res != 0)
				{
					if (res < value) value = res;
					if (value < beta) beta = value;
					if (beta <= alpha) break;
					continue;
				}

				int evaluation = predict(side, round_side == board.sides.red ? board.sides.blue : board.sides.red, movemap1, alpha, beta, max_depth - 1);
				if (evaluation < value) value = evaluation;
				if (value < beta) beta = value;
				if (beta <= alpha) break;

			}

		}
		else
		{

			value = -1000000;
			List<map_move> moves = movemap.get_moves(round_side);
			foreach (ai.map_move move in moves)
			{
				map movemap1 = apply_move(round_side, move);
				int res = movemap1.outcome(side) * 100000;

				if (res != 0)
				{
					if (res > value) value = res;
					if (value > alpha) alpha = value;
					if (beta <= alpha) break;
					continue;
				}

				int evaluation = predict(side, round_side == board.sides.red ? board.sides.blue : board.sides.red, movemap1, alpha, beta, max_depth - 1);
				if (evaluation > value) value = evaluation;
				if (value > alpha) alpha = value;
				if (beta <= alpha) break;

			}

		}
		return value;
	}

	public map_move get_best_move(board.sides side)
	{

		map_move chosen = null;
		int value = -1000000;
		int alpha = -1000000;
		int beta = 1000000;

		List<map_move> moves = get_moves(side);
		foreach (ai.map_move move in moves)
		{
			// default
			if (chosen == null)
			{
				chosen = move;
				chosen.score = -1;
			}

			map movemap = apply_move(side, move);
			int res = movemap.outcome(side);
			if (res == 1)
			{
				// win the game in current turn
				chosen = move;
				chosen.score = 1000000;
				return chosen;
			}

			int temp = predict(side, side == board.sides.red ? board.sides.blue : board.sides.red, movemap, alpha, beta);
			if (temp > value)
			{
				value = temp;
				chosen = move;
				chosen.score = value;
				if (value > alpha) alpha = value;
				if (beta <= alpha) break;
			}

		}

		return chosen;

	}

	
}


