using System;
using System.Collections.Generic;
using System.Linq;

namespace TongJi.Geometry.Computational
{
    public class Vertices : List<Vector2>
    {
        public Vertices()
        {
        }

        public Vertices(IEnumerable<Vector2> vertices)
            : base(vertices)
        {
        }
    }

    public class Triangle
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public static class MathHelper
    {
        public static float ToDegrees(float rad)
        {
            return rad / ((float)Math.PI / 180);
        }
    }

    public class Decompose
    {
        private Vector2 intersectedPoint = new Vector2(0, 0);   // store intersected point for triangulation
        private Vector2 joinerPoint = new Vector2(0, 0);    // store joiner point of 2 parallel lines
        private Vertices triangulatedVertices = new Vertices();  // used during triangulation
        private Vertices triangulated1V = new Vertices();  // used during triangulation
        private Vertices triangulated2V = new Vertices();  // used during triangulation
        private Vertices triangularVertices = new Vertices();	// fed into XNA draw call
        private int triangleCount = 0;

        private Vertices bubbleSort(Vertices v)
        {
            bool swap = true;
            while (swap)
            {
                swap = false;   // end unless found true
                for (int i = 0; i < v.Count - 1; i++)   // bubble through once
                {
                    if (v[i + 1].X < v[i].X)  // if wrong order, sort
                    {
                        Vector2 temp = v[i];
                        v[i] = v[i + 1];
                        v[i + 1] = temp;
                        swap = true;
                    }
                }   // end for
            }
            return v;
        }

        private float calculateAngle(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return Math.Abs(MathHelper.ToDegrees((float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X))
                - MathHelper.ToDegrees((float)Math.Atan2(v3.Y - v2.Y, v3.X - v2.X)));
        }

        private bool coincident(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            if (Math.Round(v1.X) == Math.Round(v3.X) && Math.Round(v1.Y) == Math.Round(v3.Y) && Math.Round(v2.X) == Math.Round(v4.X) && Math.Round(v2.Y) == Math.Round(v4.Y))
                return true;

            if (Math.Round(v1.X) == Math.Round(v4.X) && Math.Round(v1.Y) == Math.Round(v4.Y) && Math.Round(v2.X) == Math.Round(v3.X) && Math.Round(v2.Y) == Math.Round(v3.Y))
                return true;

            return false;
        }

        private int cross(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            var crossed = (v2 - v1).Kross(v3 - v2);
            if (crossed > 0)
                return 1;
            else if (crossed < 0)
                return -1;
            else return 0;
        }

        private void decomposeTrapezoid(float currentY, float nextY, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, int winding)
        {
            Vertices container = new Vertices();
            container.Add(v1);
            container.Add(v2);
            container.Add(v3);
            container.Add(v4);
            int[] sortOrder = new int[4];

            float lowerlowestX = 2000;
            float lowerhighestX = 0;
            float upperlowestX = 2000;
            float upperhighestX = 0;

            for (int i = 0; i < 4; i++)
            {
                if (Math.Round(container[i].Y) == Math.Round(nextY))    // find the lowest x value at the nextY (which is lower)
                    if (lowerlowestX > container[i].X)
                        lowerlowestX = container[i].X;

                if (Math.Round(container[i].Y) == Math.Round(nextY))    // find the highest x value at the nextY (which is lower)
                    if (lowerhighestX < container[i].X)
                        lowerhighestX = container[i].X;

                if (Math.Round(container[i].Y) == Math.Round(currentY)) // find the lowest x value at the currentY (which is higher)
                    if (upperlowestX > container[i].X)
                        upperlowestX = container[i].X;

                if (Math.Round(container[i].Y) == Math.Round(currentY)) // find the highest x value at the currentY (which is higher)
                    if (upperhighestX < container[i].X)
                        upperhighestX = container[i].X;
            }

            for (int i = 0; i < 4; i++)
            {
                if (Math.Round(container[i].X) == Math.Round(lowerlowestX) && Math.Round(container[i].Y) == Math.Round(nextY))  // bottom left
                    sortOrder[0] = i;
                if (Math.Round(container[i].X) == Math.Round(upperlowestX) && Math.Round(container[i].Y) == Math.Round(currentY))  // top left
                    sortOrder[1] = i;
                if (Math.Round(container[i].X) == Math.Round(upperhighestX) && Math.Round(container[i].Y) == Math.Round(currentY))  // top right
                    sortOrder[2] = i;
                if (Math.Round(container[i].X) == Math.Round(lowerhighestX) && Math.Round(container[i].Y) == Math.Round(nextY))  // bottom right
                    sortOrder[3] = i;
            }

            if (winding == 1)
            {
                triangulatedVertices.Add(container[sortOrder[0]]);
                triangulatedVertices.Add(container[sortOrder[1]]);
                triangulatedVertices.Add(container[sortOrder[3]]);

                triangulatedVertices.Add(container[sortOrder[1]]);
                triangulatedVertices.Add(container[sortOrder[2]]);
                triangulatedVertices.Add(container[sortOrder[3]]);
            }

            if (winding == 2)
            {
                triangulatedVertices.Add(container[sortOrder[0]]);
                triangulatedVertices.Add(container[sortOrder[1]]);
                triangulatedVertices.Add(container[sortOrder[2]]);

                triangulatedVertices.Add(container[sortOrder[0]]);
                triangulatedVertices.Add(container[sortOrder[2]]);
                triangulatedVertices.Add(container[sortOrder[3]]);
            }
        }

        private bool detectConvex(Vertices vertices)
        {
            int crossDirection = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (i == 0)
                {
                    crossDirection = cross(vertices[i], vertices[i + 1], vertices[i + 2]);
                }
                else if (i < vertices.Count - 2)
                {
                    if (crossDirection != cross(vertices[i], vertices[i + 1], vertices[i + 2]))
                        return false;
                }
                else if (i == vertices.Count - 2)
                {
                    if (crossDirection != cross(vertices[i], vertices[i + 1], vertices[0]))
                        return false;
                }
                else if (i == vertices.Count - 1)
                {
                    if (crossDirection != cross(vertices[i], vertices[0], vertices[1]))
                        return false;
                }
            }

            return true;
        }

        private Vertices greedyMerge(Vertices triangulatedVertices)
        {
            bool repeat = true;
            while (repeat)
            {
                repeat = false;
                bool breakBool = false;

                for (int i = 0; i < triangulatedVertices.Count / 3; i++)
                {
                    for (int j = 0; j < triangulatedVertices.Count / 3; j++)
                    {
                        if (j > i) // now we have two triangles to work with
                        {
                            bool isParallel = false;
                            bool isCoincident = false;
                            Vector2 joiner = new Vector2(0, 0);

                            for (int k = 0; k < 3; k++)
                            {
                                for (int l = 0; l < 3; l++)
                                {
                                    int a = i * 3 + k;  // vertex
                                    int b = a + 1;
                                    int c = j * 3 + l;
                                    int d = c + 1;

                                    if (k == 2)
                                        b -= 3;
                                    if (l == 2)
                                        d -= 3;

                                    bool nowCoincident = coincident(triangulatedVertices[a], triangulatedVertices[b], triangulatedVertices[c], triangulatedVertices[d]);

                                    if (nowCoincident)
                                        isCoincident = true;

                                    // parallel method looks for coincident vertex as well
                                    bool nowParallel = parallel(triangulatedVertices[a], triangulatedVertices[b], triangulatedVertices[c], triangulatedVertices[d]);

                                    if (nowParallel && !nowCoincident)  // don't want coincident parallel lines
                                    {
                                        joiner = joinerPoint;
                                        isParallel = true;
                                    }
                                }
                            }

                            if (isParallel == true && isCoincident == true)
                            {
                                Vertices container = new Vertices();
                                container.Add(triangulatedVertices[i * 3 + 0]);
                                container.Add(triangulatedVertices[i * 3 + 1]);
                                container.Add(triangulatedVertices[i * 3 + 2]);
                                container.Add(triangulatedVertices[j * 3 + 0]);
                                container.Add(triangulatedVertices[j * 3 + 1]);
                                container.Add(triangulatedVertices[j * 3 + 2]);

                                // remove repeat vertices
                                for (int e = 5; e >= 0; e--)
                                {
                                    for (int f = 0; f < container.Count; f++)
                                    {
                                        if (Math.Round(container[e].X) == Math.Round(container[f].X) && Math.Round(container[e].Y) == Math.Round(container[f].Y) && e != f)
                                        {
                                            container.RemoveAt(e);
                                            break;
                                        }
                                    }
                                }
                                // end remove repeat vertices

                                // now we have 4 vertices remaining, find vertex that is the joiner for parallel lines
                                for (int e = 0; e < 4; e++)
                                {
                                    if (Math.Round(container[e].X) == Math.Round(joiner.X) && Math.Round(container[e].Y) == Math.Round(joiner.Y))
                                    {
                                        container.RemoveAt(e);
                                        break;
                                    }
                                }

                                // to merge, remove all these vertices, then add them back
                                if (i > j)  // removal
                                {
                                    triangulatedVertices.RemoveAt(i * 3 + 2);
                                    triangulatedVertices.RemoveAt(i * 3 + 1);
                                    triangulatedVertices.RemoveAt(i * 3 + 0);

                                    triangulatedVertices.RemoveAt(j * 3 + 2);
                                    triangulatedVertices.RemoveAt(j * 3 + 1);
                                    triangulatedVertices.RemoveAt(j * 3 + 0);
                                }
                                else
                                {
                                    triangulatedVertices.RemoveAt(j * 3 + 2);
                                    triangulatedVertices.RemoveAt(j * 3 + 1);
                                    triangulatedVertices.RemoveAt(j * 3 + 0);

                                    triangulatedVertices.RemoveAt(i * 3 + 2);
                                    triangulatedVertices.RemoveAt(i * 3 + 1);
                                    triangulatedVertices.RemoveAt(i * 3 + 0);
                                }

                                triangulatedVertices.Add(container[0]);
                                triangulatedVertices.Add(container[1]);
                                triangulatedVertices.Add(container[2]);

                                repeat = true;
                                breakBool = true;
                                break;
                            }

                        }   // end if of two triangles
                    }
                    if (breakBool)
                        break;
                }
            }   // end while

            return triangulatedVertices;
        }

        private void insertVertexAtPosition(int position, Vector2 poi, Vertices vertices)   // edits vertices
        {
            vertices.Add(new Vector2(0, 0));
            for (int i = 0; i < vertices.Count - position - 1; i++)
            {
                vertices[vertices.Count - i - 1] = vertices[vertices.Count - i - 2];
            }
            vertices[position] = poi;
        }

        private bool intersection(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            //line 1: v1 + t1*(v2-v1)
            //line 2: v3 + t2*(v4-v3)
            float distAB, theCos, theSin, newX, ABpos;
            intersectedPoint = new Vector2(-1, -1); // reset for case of no intersection

            v2 -= v1;
            v3 -= v1;
            v4 -= v1;

            distAB = v2.Length();
            theCos = v2.X / distAB;
            theSin = v2.Y / distAB;
            newX = v3.X * theCos + v3.Y * theSin;
            v3.Y = v3.Y * theCos - v3.X * theSin; v3.X = newX;
            newX = v4.X * theCos + v4.Y * theSin;
            v4.Y = v4.Y * theCos - v4.X * theSin; v4.X = newX;

            if (v3.Y == v4.Y)
                return false;

            ABpos = v4.X + (v3.X - v4.X) * v4.Y / (v4.Y - v3.Y);

            if (ABpos > distAB || ABpos < 0)
            {
                return false;
            }

            if (v4.Y >= 0 && v3.Y >= 0)
                return false;

            if (v4.Y <= 0 && v3.Y <= 0)
                return false;

            intersectedPoint.X = v1.X + ABpos * theCos;
            intersectedPoint.Y = v1.Y + ABpos * theSin;

            return true;
        }

        private void optimalMerge(Vertices vertices)
        {
            triangulatedVertices = new Vertices();
            triangulated1V = new Vertices();

            triangulate(vertices, 1);
            triangulated1V = greedyMerge(triangulatedVertices);
            int num1 = triangulated1V.Count / 3;

            triangulatedVertices = new Vertices();
            triangulated2V = new Vertices();
            triangulate(vertices, 2);
            triangulated2V = greedyMerge(triangulatedVertices);
            int num2 = triangulated2V.Count / 3;

            if (num1 > num2)
                passThrough(triangulated2V);
            else if (num2 >= num1)
                passThrough(triangulated1V);
        }

        private bool parallel(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            float angle1, angle2;
            angle1 = 20 * MathHelper.ToDegrees((float)Math.Atan2(v2.Y - v1.Y, v2.X - v1.X));
            angle2 = 20 * MathHelper.ToDegrees((float)Math.Atan2(v4.Y - v3.Y, v4.X - v3.X));

            if (Math.Round(angle1) == Math.Round(angle2))
            {
                if (Math.Round(v1.X) == Math.Round(v3.X) && Math.Round(v1.Y) == Math.Round(v3.Y))
                {   // v1 = v3
                    joinerPoint = v1;
                    return true;
                }

                if (Math.Round(v1.X) == Math.Round(v4.X) && Math.Round(v1.Y) == Math.Round(v4.Y))
                {   // v1 = v4
                    joinerPoint = v1;
                    return true;
                }

                if (Math.Round(v2.X) == Math.Round(v3.X) && Math.Round(v2.Y) == Math.Round(v3.Y))
                {   // v2 = v3
                    joinerPoint = v2;
                    return true;
                }

                if (Math.Round(v2.X) == Math.Round(v4.X) && Math.Round(v2.Y) == Math.Round(v4.Y))
                {   // v2 = v4
                    joinerPoint = v2;
                    return true;
                }
            }

            angle1 = 20 * MathHelper.ToDegrees((float)Math.Atan2(v1.Y - v2.Y, v1.X - v2.X));
            if (Math.Round(angle1) == Math.Round(angle2))
            {
                if (Math.Round(v1.X) == Math.Round(v3.X) && Math.Round(v1.Y) == Math.Round(v3.Y))
                {   // v1 = v3
                    joinerPoint = v1;
                    return true;
                }

                if (Math.Round(v1.X) == Math.Round(v4.X) && Math.Round(v1.Y) == Math.Round(v4.Y))
                {   // v1 = v4
                    joinerPoint = v1;
                    return true;
                }

                if (Math.Round(v2.X) == Math.Round(v3.X) && Math.Round(v2.Y) == Math.Round(v3.Y))
                {   // v2 = v3
                    joinerPoint = v2;
                    return true;
                }

                if (Math.Round(v2.X) == Math.Round(v4.X) && Math.Round(v2.Y) == Math.Round(v4.Y))
                {   // v2 = v4
                    joinerPoint = v2;
                    return true;
                }
            }
            return false;
        }

        private void passThrough(Vertices vPass)
        {
            for (int i = 0; i < vPass.Count; i++)
            {
                triangularVertices.Add(vPass[i]);
            }
            triangleCount += vPass.Count / 3;
        }

        private Vertices seidelVertices(Vertices vertices)        // adds vertices into 'vertices'
        {
            int numLines = vertices.Count;
            float[] lineYcoords = new float[numLines];
            int yCount = 0;

            for (int i = 0; i < numLines; i++)
            {
                lineYcoords[i] = vertices[i].Y;
            }

            bool done = false;
            bool repeatY = false;

            while (done == false)   // done when no more vertices need to be added
            {
                repeatY = false;    // don't repeat by default
                for (int i = 0; i < vertices.Count; i++)
                {
                    // draw horz line and detect intersection
                    // remember one more intersection: the last vertex to first
                    bool checkInter;

                    if (i != vertices.Count - 1)    // if not the last vertex
                        checkInter = intersection(vertices[i], vertices[i + 1], new Vector2(0, lineYcoords[yCount]), new Vector2(1024, lineYcoords[yCount]));
                    else // last vertex
                        checkInter = intersection(vertices[i], vertices[0], new Vector2(0, lineYcoords[yCount]), new Vector2(1024, lineYcoords[yCount]));

                    if (checkInter)
                    {
                        bool contains = false;
                        for (int j = 0; j < vertices.Count; j++)
                        {
                            if (Math.Round(vertices[j].X) == Math.Round(intersectedPoint.X) && Math.Round(vertices[j].Y) == Math.Round(intersectedPoint.Y))
                            {
                                contains = true;
                            }
                        }

                        if (!contains)       // if point not contained, add it in and reloop
                        {
                            insertVertexAtPosition(i + 1, intersectedPoint, vertices);
                            repeatY = true;
                            break;
                        }

                    }   // end if

                }       // end for

                if (!repeatY)   // if don't repeat, move on
                {
                    yCount++;
                }

                if (yCount == numLines)
                    done = true;

            }   // end while

            return vertices;
        }

        private void triangulate(Vertices vertices, int winding)
        {
            int numLines = vertices.Count;
            float[] lineYcoords = new float[numLines];

            for (int i = 0; i < numLines; i++)
            {
                lineYcoords[i] = vertices[i].Y; // initialise values
            }

            // bubble sort y values
            bool swap = true;
            while (swap)
            {
                swap = false;   // end unless found true
                for (int i = 0; i < numLines - 1; i++)   // bubble through once
                {
                    if (lineYcoords[i + 1] < lineYcoords[i])  // if wrong order, sort
                    {
                        float temp = lineYcoords[i];
                        lineYcoords[i] = lineYcoords[i + 1];
                        lineYcoords[i + 1] = temp;
                        swap = true;    // swap done, requires next iteration
                    }
                }   // end for
            }
            //  end bubble sort

            float currentY = 2000;
            float nextY = 2000;

            // here we begin
            for (int i = 0; i < lineYcoords.Length - 1; i++)    // lineYcoords.Length - 1 is the last vertex
            {

                if (Math.Round(lineYcoords[i]) == Math.Round(lineYcoords[i + 1]))   // if next position y is the same
                {
                    continue;
                }
                currentY = lineYcoords[i];
                nextY = lineYcoords[i + 1];

                // y values in ascending order in currentY

                // --------------------------- between rows y1 and y2 ---------------------------
                // create list y1 and y2
                Vertices y1 = new Vertices();   // currentY
                Vertices y2 = new Vertices();   // nextY

                // store all required vertices in lists y1 and y2 for currentY and nextY
                for (int j = 0; j < vertices.Count; j++)        // vertices.Count - 1 is the last vertex
                {
                    if (j != vertices.Count - 1)    // if not last vertex
                    {
                        if (Math.Round(vertices[j].Y) == Math.Round(currentY) && Math.Round(vertices[j + 1].Y) == Math.Round(nextY))
                        {
                            y1.Add(vertices[j]);
                            y2.Add(vertices[j + 1]);
                        }
                        if (Math.Round(vertices[j].Y) == Math.Round(nextY) && Math.Round(vertices[j + 1].Y) == Math.Round(currentY))
                        {
                            y1.Add(vertices[j + 1]);
                            y2.Add(vertices[j]);
                        }
                    }
                    else     // if last vertex, line to origin point
                    {
                        if (Math.Round(vertices[j].Y) == Math.Round(currentY) && Math.Round(vertices[0].Y) == Math.Round(nextY))
                        {
                            y1.Add(vertices[j]);
                            y2.Add(vertices[0]);
                        }
                        if (Math.Round(vertices[j].Y) == Math.Round(nextY) && Math.Round(vertices[0].Y) == Math.Round(currentY))
                        {
                            y1.Add(vertices[0]);
                            y2.Add(vertices[j]);
                        }
                    }
                }

                // sort y1 and y2 in ascending order of x
                y1 = bubbleSort(y1);
                y2 = bubbleSort(y2);

                // take 2 vertices from y1 and 2 vertices from y2
                if (y1.Count % 2 == 0)  // to prevent out of bounds exceptions
                {
                    for (int j = 0; j < y1.Count; j += 2)
                    {
                        if (Math.Round(y1[j].X) == Math.Round(y1[j + 1].X) && Math.Round(y1[j].Y) == Math.Round(y1[j + 1].Y)) // notch at y1
                        {
                            triangulatedVertices.Add(y1[j]);
                            triangulatedVertices.Add(y2[j]);
                            triangulatedVertices.Add(y2[j + 1]);
                        }
                        else if (Math.Round(y2[j].X) == Math.Round(y2[j + 1].X) && Math.Round(y2[j].Y) == Math.Round(y2[j + 1].Y))  // notch at y2
                        {
                            triangulatedVertices.Add(y2[j]);
                            triangulatedVertices.Add(y1[j]);
                            triangulatedVertices.Add(y1[j + 1]);
                        }
                        else // no notches, therefore trapezoid
                        {
                            decomposeTrapezoid(currentY, nextY, y1[j], y1[j + 1], y2[j], y2[j + 1], winding);
                        }
                    }
                }
                // --------------------------- between y1 and y2 ---------------------------
            }   // end for
        }

        public List<Triangle> Run(Vertices vertices)
        {
            optimalMerge(vertices);
            List<Triangle> triangles = new List<Triangle>();
            for (int i = 0; i < triangularVertices.Count; i += 3)
            {
                triangles.Add(new Triangle(triangularVertices[i], triangularVertices[i + 1], triangularVertices[i + 2]));
            }
            return triangles;
        }
    }
}
