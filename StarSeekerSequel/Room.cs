using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class Room: IDrawable{
	private List<GameObject> things {get; set;}
	private Dictionary<string, GameObject> thingsByName { get; set; }
	public Camera cam;
	public Player player;

	public Room()
	{
		things = new List<GameObject>();
		thingsByName = new Dictionary<string, GameObject>();
		cam = new Camera(0, 0, 500, 300);
	}
	public Room(string filename) : this()
	{
		PopulateRoomFromFile(filename);
	}

	public void Update(GameTime gt)
	{
		for (int i = 0; i < things.Count; i++)
		{
			things[i].Update(gt);
		}
		cam.X = (int)MathF.Floor(player.X + 0.5f) - 250;
		cam.Y = (int)MathF.Floor(player.Y + 0.5f) - 150;
	}
	public void Draw(Camera c)
	{
		SortObjects();
		for(int i = 0; i < things.Count; i++)
		{
			things[i].Draw(c);
		}
		Console.WriteLine("done drawing");
	}

	public void RegisterObject(GameObject go)
	{
		if (!(things.Contains(go)))
		{
			things.Add(go);
			if (thingsByName.ContainsKey(go.GUID))
			{
				throw new System.Exception("Room thing list was missing object, but the dictionary already had it. What gives?!");
			}
			else
			{
				thingsByName.Add(go.GUID, go);
			}
			SortObjects();
		}
	}
	public void RegisterPlayer(Player p)
	{
		if (!(things.Contains(p)))
		{
			things.Add(p);
			if (thingsByName.ContainsKey(p.GUID))
			{
				throw new System.Exception("Room thing list was missing player, but the dictionary already had it. What gives?!");
			}
			else
			{
				this.player = p;
				thingsByName.Add(p.GUID, p);
			}
			SortObjects();
		}
		
	}
	public void DeleteObjectByGUID(string guid)
	{
		GameObject goToDelete = null;
		thingsByName.TryGetValue(guid, out goToDelete);
		if(goToDelete != null)
		{
			things.Remove(goToDelete);
			thingsByName.Remove(guid);
		}
	}
	private void SortObjects()
	{
		things.Sort(delegate (GameObject a, GameObject b) {
			if (a == null && b == null)
			{
				return 0;
			}
			else if (a == null)
			{
				return -1;
			}
			else if (b == null)
			{
				return 1;
			}
			else
			{
				int comparison = a.Z.CompareTo(b.Z);
				if (comparison == 0)
				{
					return (a.Y.CompareTo(b.Y));
				}
				else
				{
					return comparison;
				}
			}
		});
	}
	public List<Collider> AllColliders()
	{
		List<Collider> colls = new List<Collider>();
		foreach(GameObject go in things)
		{
			if(go.coll != null)
			{
				colls.Add(go.coll);
			}
		}
		return colls;
	}



	public void PopulateRoomFromFile(string datafile)
	{
		//string adaptiveFull = Path.GetFullPath(datafile);
		//string fullpath = @"C:\Users\djhol\source\repos\StarSeekerSequel\StarSeekerSequel\" + datafile;
		string fullpath = @"..\..\..\" + datafile;
		if (File.Exists(fullpath))
		{
			using (StreamReader reader = new StreamReader(File.OpenRead(fullpath)))
			{
				string roomdata = reader.ReadToEnd();
				RoomDataTemplate raw = JsonConvert.DeserializeObject<RoomDataTemplate>(roomdata);
				System.Diagnostics.Debug.WriteLine("blah");
				foreach(GameObjectTemplate got in raw.things)
				{
					GameObject go = new GameObject(got.id, new Vector3(got.x, got.y, got.z), null);
					if(got.coll == "rect")
					{
						go.GiveRectCollider(new Vector2(got.cw, got.ch));
					}else if(got.coll == "circle")
					{
						go.GiveCircleCollider(got.cr);
					}
					this.RegisterObject(go);
				}
			}
		}
		else
		{
			System.Diagnostics.Debug.WriteLine("uhoh");
		}
	}
	class RoomDataTemplate
	{
		public IList<GameObjectTemplate> things { get; set; }
	}
	class GameObjectTemplate {
		public string id { get; set; }
		public float x { get; set; }
		public float y { get; set; }
		public float z { get; set; }
		public string coll { get; set; }
		public float cw { get; set; }
		public float ch { get; set; }
		public float cr { get; set; }
	}
}