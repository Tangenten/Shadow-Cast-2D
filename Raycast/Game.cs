using System;
using System.Collections.Generic;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace netcore {
	public class Game {
		private int windowHeight;
		private int windowWidth;

		private readonly RenderWindow window;
		private readonly Clock clock;
		private float deltaTime;

		public Game() {
			this.clock = new Clock();
			this.deltaTime = 0.0f;

			this.windowWidth = 1920;
			this.windowHeight = 1080;

			this.window = new RenderWindow(new VideoMode((uint) this.windowWidth, (uint) this.windowHeight), "Light and Shadow", Styles.Default);
			this.window.SetVerticalSyncEnabled(true);

			this.window.Closed += this.OnClose;
			this.window.Resized += this.OnResize;
		}


		private void OnClose(object? sender, EventArgs e) {
			this.window.Close();
		}

		private void OnResize(object sender, SizeEventArgs e) {
			this.windowWidth = (int) e.Width;
			this.windowHeight = (int) e.Height;
		}

		public void Start() {
			this.Run();
		}

		public void Stop() {
			this.window.Close();
		}

		public void Run() {
			List<RectangleShape> gridLines = this.generateBackgroundGrid(32);
			List<RectangleShape> walls = this.generateWalls();

			RenderStates renderState = new RenderStates(BlendMode.None);
			RenderTexture renderTexture = new RenderTexture((uint) this.windowWidth, (uint) this.windowHeight);
			Sprite renderSprite = new Sprite();

			while (this.window.IsOpen) {
				this.deltaTime = this.clock.Restart().AsSeconds();
				//Console.WriteLine(1f / this.deltaTime);

				this.window.DispatchEvents();

				this.window.Clear(Color.Black);
				renderTexture.Clear(Color.Black);

				for (int i = 0; i < gridLines.Count; i++) {
					this.window.Draw(gridLines[i], new RenderStates(BlendMode.Alpha));
				}

				for (int i = 0; i < walls.Count; i++) {
					this.window.Draw(walls[i], new RenderStates(BlendMode.Alpha));
				}

				Vector2f mousePosition = (Vector2f) Mouse.GetPosition(this.window);
				VertexBuffer mask = this.getLightMask3(mousePosition, walls);

				/*List<Vector2f> points = this.getLightMask2(mousePosition, walls);
				for (int i = 0; i < points.Count; i++) {
					CircleShape circle = new CircleShape(2);
					circle.FillColor = Color.Red;
					circle.Position = points[i] - new Vector2f(2, 2);
					//window.Draw(circle);

					Vertex[] cornerLine = new Vertex[2];
					cornerLine[0].Position = mousePosition;
					cornerLine[0].Color = new Color(255, 255, 255, 64);
					cornerLine[1].Position = points[i];
					cornerLine[1].Color = new Color(255, 255, 255, 64);
					this.window.Draw(cornerLine, PrimitiveType.LineStrip);
				}*/

				renderTexture.Draw(mask, renderState);
				renderTexture.Display();

				renderSprite.Texture = renderTexture.Texture;
				this.window.Draw(renderSprite);

				this.window.Display();
			}
		}

		public List<RectangleShape> generateBackgroundGrid(int sections) {
			List<RectangleShape> gridLines = new List<RectangleShape>();

			float gridGapWidth = (float) this.windowWidth / sections;
			float gridGapHeight = (float) this.windowHeight / sections;

			float lineThickness = 1;
			Color lineColor = new Color(255, 255, 255, 127);

			for (float i = 0; i < this.windowHeight; i += gridGapHeight) {
				RectangleShape line = new RectangleShape(new Vector2f(this.windowWidth, lineThickness));
				line.Origin = new Vector2f(lineThickness / 2, lineThickness / 2);
				line.Position = new Vector2f(0, i);
				line.FillColor = lineColor;

				gridLines.Add(line);
			}

			for (float i = 0; i < this.windowWidth; i += gridGapWidth) {
				RectangleShape line = new RectangleShape(new Vector2f(lineThickness, this.windowHeight));
				line.Origin = new Vector2f(lineThickness / 2, lineThickness / 2);
				line.Position = new Vector2f(i, 0);
				line.FillColor = lineColor;

				gridLines.Add(line);
			}

			return gridLines;
		}

		public List<RectangleShape> generateWalls() {
			List<RectangleShape> walls = new List<RectangleShape>();

			float lineThickness = 16;
			Color lineColor = new Color(0, 255, 0, 255);

			float gridGapWidth = (float) this.windowWidth / 16;
			float gridGapHeight = (float) this.windowHeight / 16;

			RectangleShape wall = new RectangleShape(new Vector2f(this.windowWidth, lineThickness));
			wall.Position = new Vector2f(0, 0);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(this.windowWidth, lineThickness));
			wall.Position = new Vector2f(0, this.windowHeight - lineThickness);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(lineThickness, this.windowHeight));
			wall.Position = new Vector2f(0, 0);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(lineThickness, this.windowHeight));
			wall.Position = new Vector2f(this.windowWidth - lineThickness, 0);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(gridGapWidth * 4, lineThickness));
			wall.Position = new Vector2f(0, gridGapHeight);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(gridGapWidth * 4, lineThickness));
			wall.Position = new Vector2f(gridGapWidth * 8, gridGapHeight * 4);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(gridGapWidth * 4, gridGapWidth * 1));
			wall.Position = new Vector2f(gridGapWidth * 6, gridGapHeight * 10);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(lineThickness, gridGapHeight * 0.25f));
			wall.Position = new Vector2f(gridGapWidth * 13, gridGapHeight * 6);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(lineThickness, gridGapHeight * 0.25f));
			wall.Position = new Vector2f(gridGapWidth * 13, gridGapHeight * 6.5f);
			wall.FillColor = lineColor;
			walls.Add(wall);

			wall = new RectangleShape(new Vector2f(lineThickness, gridGapHeight * 0.25f));
			wall.Position = new Vector2f(gridGapWidth * 13, gridGapHeight * 7);
			wall.FillColor = lineColor;
			walls.Add(wall);

			return walls;
		}

		public List<List<Vector2f>> getWallLines(List<RectangleShape> walls) {
			List<List<Vector2f>> wallLines = new List<List<Vector2f>>(walls.Count * 4);

			for (int i = 0; i < walls.Count; i++) {
				Vector2f wallCorner = new Vector2f(walls[i].Position.X, walls[i].Position.Y);
				for (uint j = 0; j < walls[i].GetPointCount(); j++) {
					List<Vector2f> line = new List<Vector2f>(2);
					line.Add(wallCorner + walls[i].GetPoint(j));
					line.Add(wallCorner + walls[i].GetPoint((j + 1) % walls[i].GetPointCount()));
					wallLines.Add(line);
				}
			}

			return wallLines;
		}

		public List<Vector2f> getWallCorners(List<RectangleShape> walls) {
			List<Vector2f> wallCorners = new List<Vector2f>(walls.Count * 4);

			for (int i = 0; i < walls.Count; i++) {
				Vector2f wallCorner = new Vector2f(walls[i].Position.X, walls[i].Position.Y);
				for (uint j = 0; j < walls[i].GetPointCount(); j++) {
					wallCorners.Add(wallCorner + walls[i].GetPoint(j));
				}
			}

			return wallCorners;
		}

		public List<Vector2f> getLightMask(Vector2f position, List<RectangleShape> walls) {
			List<Vector2f> allIntersections = new List<Vector2f>();
			List<List<Vector2f>> wallLines = this.getWallLines(walls);

			for (float i = 0; i < 360; i += 0.5f) {
				Vector2f normalVector = new Vector2f((float) Math.Sin(this.DegreeToRadian(i)), (float) -Math.Cos(this.DegreeToRadian(i)));
				List<Vector2f> intersections = this.rayCast(position, normalVector, wallLines);

				if (intersections.Count > 0) allIntersections.Add(this.getClosestPoint(position, intersections));
			}

			return allIntersections;
		}

		public List<Vector2f> getLightMask2(Vector2f position, List<RectangleShape> walls) {
			List<(Vector2f, float)> allIntersections = new List<(Vector2f, float)>();

			List<List<Vector2f>> wallLines = this.getWallLines(walls);
			List<Vector2f> wallCorners = this.getWallCorners(walls);

			for (int i = 0; i < wallCorners.Count; i++) {
				Vector2f normalVector = this.VectorBetweenPoints(position, wallCorners[i]);
				List<Vector2f> intersections = this.rayCast(position, normalVector, wallLines);

				if (intersections.Count > 0) allIntersections.Add((this.getClosestPoint(position, intersections), this.VectorToDegree(normalVector)));
			}

			allIntersections.Sort((a, b) => a.Item2.CompareTo(b.Item2));

			List<Vector2f> allIntersectionsSorted = allIntersections.Select(_ => _.Item1).ToList();

			return allIntersectionsSorted;
		}

		public VertexBuffer getLightMask3(Vector2f position, List<RectangleShape> walls) {
			List<(Vector2f, float)> allIntersections = new List<(Vector2f, float)>();

			List<List<Vector2f>> wallLines = this.getWallLines(walls);
			List<Vector2f> wallCorners = this.getWallCorners(walls);

			for (int i = 0; i < wallCorners.Count; i++) {
				Vector2f normalVector = this.VectorBetweenPoints(position, wallCorners[i]);
				List<Vector2f> intersections = new List<Vector2f>();
				//intersections = this.rayCast(position, normalVector, wallLines);

				//if (intersections.Count > 0) allIntersections.Add((this.getClosestPoint(position, intersections), this.VectorToDegree(normalVector)));

				Vector2f normalVector2 = this.RotatePointAroundPoint(normalVector, position, 0.0001f);
				intersections = this.rayCast(position, normalVector2, wallLines);

				if (intersections.Count > 0) allIntersections.Add((this.getClosestPoint(position, intersections), this.VectorToDegree(normalVector2)));

				Vector2f normalVector3 = this.RotatePointAroundPoint(normalVector, position, -0.0001f);
				intersections = this.rayCast(position, normalVector3, wallLines);

				if (intersections.Count > 0) allIntersections.Add((this.getClosestPoint(position, intersections), this.VectorToDegree(normalVector3)));

				/*for (int j = 0; j < intersections.Count; j++)
				{
				    allIntersections.Add((intersections[j], VectorToDegree(normalVector)));
				}*/
			}

			allIntersections.Sort((a, b) => a.Item2.CompareTo(b.Item2));

			List<Vector2f> allIntersectionsSorted = allIntersections.Select(_ => _.Item1).ToList();

			Vertex[] vertices = new Vertex[allIntersectionsSorted.Count + 2];
			vertices[0] = new Vertex(position);
			vertices[0].Color = Color.Transparent;

			for (int i = 0; i < allIntersectionsSorted.Count; i++) {
				vertices[i + 1] = new Vertex(allIntersectionsSorted[i]);
				vertices[i + 1].Color = new Color(4, 0, 0, 255);
			}

			vertices[vertices.Length - 1] = vertices[1];
			vertices[vertices.Length - 1].Color = new Color(4, 0, 0, 255);

			VertexBuffer vertexBuffer = new VertexBuffer((uint) allIntersectionsSorted.Count, PrimitiveType.TriangleFan, VertexBuffer.UsageSpecifier.Dynamic);
			vertexBuffer.Update(vertices);

			return vertexBuffer;
		}

		public List<Vector2f> rayCast(Vector2f position, Vector2f direction, List<List<Vector2f>> lines) {
			List<Vector2f> intersections = new List<Vector2f>();

			for (int i = 0; i < lines.Count; i++) {
				float x1 = lines[i][0].X;
				float y1 = lines[i][0].Y;
				float x2 = lines[i][1].X;
				float y2 = lines[i][1].Y;

				float x3 = position.X;
				float y3 = position.Y;
				float x4 = position.X + direction.X;
				float y4 = position.Y + direction.Y;

				float den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

				if (den != 0f) // if not perpendicular
				{
					float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
					float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;

					if (t > 0f && t < 1f && u > 0f) // if intersection, if in direction of direction
					{
						float x = x1 + t * (x2 - x1);
						float y = y1 + t * (y2 - y1);

						intersections.Add(new Vector2f(x, y));
					}
				}
			}
			return intersections;
		}

		public Vector2f getClosestPoint(Vector2f origin, List<Vector2f> points) {
			float distance = float.MaxValue;
			int index = 0;

			for (int i = 0; i < points.Count; i++) {
				float min = this.distanceBetweenPoints(origin, points[i]);
				if (min < distance) {
					distance = min;
					index = i;
				}
			}

			return points[index];
		}

		public float distanceBetweenPoints(Vector2f p1, Vector2f p2) {
			return (float) Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
		}

		public Vector2f VectorBetweenPoints(Vector2f p1, Vector2f p2) {
			Vector2f distance = p2 - p1;
			//var norm = (float)Math.Sqrt(Math.Pow(distance.X, 2) + Math.Pow(distance.Y, 2));
			//var direction = new Vector2f(distance.X / norm, distance.Y / norm);
			//var normVector = new Vector2f((float) (direction.X * Math.Sqrt(2)), (float) (direction.Y * Math.Sqrt(2)));

			return distance;
		}

		public float DegreeToRadian(float degree) {
			return (float) (Math.PI * degree / 180f);
		}

		public float VectorToDegree(Vector2f v) {
			return (float) Math.Atan2(v.X, v.Y);
		}

		public Vector2f RotatePointAroundPoint(Vector2f point, Vector2f origin, float radian) {
			float x = point.X - origin.X;
			float y = point.Y - origin.Y;

			float x2 = (float) (x * Math.Cos(radian) - y * Math.Sin(radian));
			float y2 = (float) (x * Math.Sin(radian) + y * Math.Cos(radian));

			x2 = x2 + origin.X;
			y2 = y2 + origin.Y;

			Vector2f v = new Vector2f(x2, y2);

			return v;
		}
	}
}
