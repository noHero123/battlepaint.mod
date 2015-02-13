using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using System.Reflection;


namespace battlepaint.mod
{
    public struct paintdata
    {
        public Vector2 vector;
        public Color color;

        public paintdata(Vector2 v, Color c)
        {
            this.vector = v;
            this.color = c;
        }
    }


    public class Screendrawer : ICommListener
    {

        List<paintdata> linePoints = new List<paintdata>();
        public float threshold = 0.001f;
        Vector3 lastPos = Vector3.one * float.MaxValue;

        public Color currentColor = Color.green;

        public Screendrawer()
        {
            try
            {
                App.Communicator.addListener(this);
            }
            catch { }

        }

        public void handleMessage(Message msg)
        { // collect data for enchantments (or units who buff)

            if (msg is RoomChatMessageMessage)
            {
                RoomChatMessageMessage rcmm = (RoomChatMessageMessage)msg;
                if (rcmm.text.StartsWith("You have joined"))
                {
                    WhisperMessage nwm = new WhisperMessage();
                    nwm.from = "Summoner";
                    nwm.text = "<color=#a59585>Battlepainter:</color> " + "you are able to paint with your mouse during every battle! in PTP-Mode you have to additionally press P to draw. A right-click will delete the painting.";

                    App.ArenaChat.handleMessage(nwm);
                    App.Communicator.removeListener(this);

                }
            }

            return;
        }


        public void onConnect(OnConnectData ocd)
        {
            return; // don't care
        }


        public void Update(bool p)
        {
            saveMousestuff(p);

            
        }

        void saveMousestuff(bool clickp)
        {
            UpdateLine();


            if (Input.GetButton("Fire1") && ((clickp && Input.GetKey(KeyCode.P)) || !clickp))
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 0;

                float dist = Vector3.Distance(lastPos, mousePos);

                if (dist <= threshold)
                    return;

                lastPos = mousePos;
                if (linePoints == null)
                    linePoints = new List<paintdata>();
                mousePos.y = Screen.height - mousePos.y;
                linePoints.Add(new paintdata(mousePos,this.currentColor));


            }
            else
            {
                if (linePoints.Count >= 1 && linePoints[linePoints.Count - 1].vector.x != -1000)
                {
                    linePoints.Add(new paintdata(new Vector2(-1000, 0), this.currentColor));
                }
            }

            if (Input.GetButton("Fire2"))
            {
                linePoints.Clear();
            }
        }

        void UpdateLine()
        {
            for (int i = 0; i < linePoints.Count-1; i++)
            {
                if (linePoints[i].vector.x == -1000)
                {
                    continue;
                }
                if (linePoints[i + 1].vector.x == -1000) 
                {
                    i+=2;
                    continue;
                }
                //Console.WriteLine("draw line from " + linePoints[i] + " to " + linePoints[i+1] );
                Drawing.DrawLine(linePoints[i].vector, linePoints[i + 1].vector, linePoints[i].color, 3, false);
            }
        }


    }

    //source for drawing
    // http://answers.unity3d.com/questions/186601/display-a-line-in-gui-without-texture-gui-line-.html

    public static class Drawing
    {
        private static Texture2D aaLineTex = null;
        private static Texture2D lineTex = null;
        private static Material blitMaterial = null;
        private static Material blendMaterial = null;
        private static Rect lineRect = new Rect(0, 0, 1, 1);

        // Draw a line in screen space, suitable for use from OnGUI calls from either
        // MonoBehaviour or EditorWindow. Note that this should only be called during repaint
        // events, when (Event.current.type == EventType.Repaint).
        //
        // Works by computing a matrix that transforms a unit square -- Rect(0,0,1,1) -- into
        // a scaled, rotated, and offset rectangle that corresponds to the line and its width.
        // A DrawTexture call used to draw a line texture into the transformed rectangle.
        //
        // More specifically:
        //      scale x by line length, y by line width
        //      rotate around z by the angle of the line
        //      offset by the position of the upper left corner of the target rectangle
        //
        // By working out the matrices and applying some trigonometry, the matrix calculation comes
        // out pretty simple. See https://app.box.com/s/xi08ow8o8ujymazg100j for a picture of my
        // notebook with the calculations.
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
        {
            // Normally the static initializer does this, but to handle texture reinitialization
            // after editor play mode stops we need this check in the Editor.
    #if UNITY_EDITOR
         if (!lineTex)
         {
             Initialize();
         }
    #endif

            // Note that theta = atan2(dy, dx) is the angle we want to rotate by, but instead
            // of calculating the angle we just use the sine (dy/len) and cosine (dx/len).
            float dx = pointB.x - pointA.x;
            float dy = pointB.y - pointA.y;
            float len = Mathf.Sqrt(dx * dx + dy * dy);

            // Early out on tiny lines to avoid divide by zero.
            // Plus what's the point of drawing a line 1/1000th of a pixel long??
            if (len < 0.001f)
            {
                return;
            }

            // Pick texture and material (and tweak width) based on anti-alias setting.
            Texture2D tex;
            Material mat;
            if (antiAlias)
            {
                // Multiplying by three is fine for anti-aliasing width-1 lines, but make a wide "fringe"
                // for thicker lines, which may or may not be desirable.
                width = width * 3.0f;
                tex = aaLineTex;
                mat = blendMaterial;
            }
            else
            {
                tex = lineTex;
                mat = blitMaterial;
            }

            float wdx = width * dy / len;
            float wdy = width * dx / len;

            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.m00 = dx;
            matrix.m01 = -wdx;
            matrix.m03 = pointA.x + 0.5f * wdx;
            matrix.m10 = dy;
            matrix.m11 = wdy;
            matrix.m13 = pointA.y - 0.5f * wdy;

            // Use GL matrix and Graphics.DrawTexture rather than GUI.matrix and GUI.DrawTexture,
            // for better performance. (Setting GUI.matrix is slow, and GUI.DrawTexture is just a
            // wrapper on Graphics.DrawTexture.)
            GL.PushMatrix();
            GL.MultMatrix(matrix);
            Graphics.DrawTexture(lineRect, tex, lineRect, 0, 0, 0, 0, color, mat);
            GL.PopMatrix();
        }

        // Other than method name, DrawBezierLine is unchanged from Linusmartensson's original implementation.
        public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
        {
            Vector2 lastV = CubeBezier(start, startTangent, end, endTangent, 0);
            for (int i = 1; i < segments; ++i)
            {
                Vector2 v = CubeBezier(start, startTangent, end, endTangent, i / (float)segments);
                Drawing.DrawLine(lastV, v, color, width, antiAlias);
                lastV = v;
            }
        }

        private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
        {
            float rt = 1 - t;
            return rt * rt * rt * s + 3 * rt * rt * t * st + 3 * rt * t * t * et + t * t * t * e;
        }

        // This static initializer works for runtime, but apparently isn't called when
        // Editor play mode stops, so DrawLine will re-initialize if needed.
        static Drawing()
        {
            Initialize();
        }

        private static void Initialize()
        {
            if (lineTex == null)
            {
                lineTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                lineTex.SetPixel(0, 1, Color.white);
                lineTex.Apply();
            }
            if (aaLineTex == null)
            {
                // TODO: better anti-aliasing of wide lines with a larger texture? or use Graphics.DrawTexture with border settings
                aaLineTex = new Texture2D(1, 3, TextureFormat.ARGB32, false);
                aaLineTex.SetPixel(0, 0, new Color(1, 1, 1, 0));
                aaLineTex.SetPixel(0, 1, Color.white);
                aaLineTex.SetPixel(0, 2, new Color(1, 1, 1, 0));
                aaLineTex.Apply();
            }

            // GUI.blitMaterial and GUI.blendMaterial are used internally by GUI.DrawTexture,
            // depending on the alphaBlend parameter. Use reflection to "borrow" these references.
            blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
            blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
        }
    }




}


