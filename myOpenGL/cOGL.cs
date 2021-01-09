using System;
using System.Collections.Generic;
using System.Windows.Forms;

using System.Drawing;



namespace OpenGL
{
    class cOGL
    {

        GLUquadric obj;
        GLUquadric obj2;

        GLUquadric quad;


        public float[] ScrollValue = new float[14];
        public bool checkBox = false;



        float flap = 1;
        public double speed = 0;
        public Boolean speedFlg = true;

        public double speed_inc = 1;
        public double spiral_inc = 1;
        public double scale_inc = 3;

        public double speedLim = 50;
        public double spindLim = 180;

        int f = 0;
        int spirala = 0;
        public Boolean spiralaFlg = false;
        public Boolean forward = true;

        double hh = 1;
        double headMov = 1;
        double tilt = 1;


        double updown = 1;
        double ascend = 1;
        bool tiltFlg = false;
        bool heightFlg = false;
        bool spirlFlg = false;
        bool flapFlg = false;

        public double lake_size = 160;



        Control p;
        int Width;
        int Height;

        public cOGL(Control pb)
        {
            p = pb;
            Width = p.Width;
            Height = p.Height;
            obj = GLU.gluNewQuadric();

            InitializeGL();

            //3 points of ground plane
            ground[0, 0] = 1;
            ground[0, 1] = 0;
            ground[0, 2] = 0;

            ground[1, 0] = -1;
            ground[1, 1] = 0;
            ground[1, 2] = 0;

            ground[2, 0] = 0;
            ground[2, 1] = 0;
            ground[2, 2] = -1;

            //light position

            pos[0] = light_position[0] = ScrollValue[9];
            pos[1] = light_position[1] = ScrollValue[10];
            pos[2] = light_position[2] = ScrollValue[11];
            pos[3] = light_position[3] = 0;


            light_position_reflected[0] = -ScrollValue[9];
            light_position_reflected[1] = -ScrollValue[10];
            light_position_reflected[2] = -ScrollValue[11];
            light_position_reflected[3] = 0;


            GL.glLightfv(GL.GL_LIGHT0, GL.GL_AMBIENT, light_ambient);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_DIFFUSE, light_diffuse);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_SPECULAR, light_specular);
            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_position);

            GL.glLightfv(GL.GL_LIGHT1, GL.GL_AMBIENT, light_ambient);
            GL.glLightfv(GL.GL_LIGHT1, GL.GL_DIFFUSE, light_diffuse);
            GL.glLightfv(GL.GL_LIGHT1, GL.GL_SPECULAR, light_specular);
            GL.glLightfv(GL.GL_LIGHT1, GL.GL_POSITION, light_position_reflected);
            GL.glLightModelfv(GL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);


        }

        ~cOGL()
        {
            WGL.wglDeleteContext(m_uint_RC);
        }

        uint m_uint_HWND = 0;

        public uint HWND
        {
            get { return m_uint_HWND; }
        }

        uint m_uint_DC = 0;

        public uint DC
        {
            get { return m_uint_DC; }
        }
        uint m_uint_RC = 0;

        public uint RC
        {
            get { return m_uint_RC; }
        }


        public float zShift = 0.0f;
        public float yShift = 0.0f;
        public float xShift = 0.0f;

        public float zAngle = 0.0f;
        public float yAngle = 0.0f;
        public float xAngle = 0.0f;
        public int intOptionC = 0;

        //Light properties..
        float[] light_ambient = { 0.0f, 0.0f, 0.0f, 1.0f };
        float[] light_diffuse = { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] light_specular = { 1.0f, 1.0f, 1.0f, 1.0f };
        float[] light_position = { 0.0f, 100.0f, 0.0f, 0.0f };
        float[] light_position_reflected = { 0.0f, 100.0f, 0.0f, 0.0f };
        float[] lmodel_ambient = { 0.4f, 0.4f, 0.4f, 1.0f };


        public float moon = 0;
        public float planet = 0;

        float[] cubeXform = new float[16];
        float[] planeCoeff = { 1, 1, 1, 1 };
        float[,] ground = new float[3, 3];

        public float[] pos = new float[4];




        public void Drawlake()
        {
            float[] lake_ambuse = { 0.0117f, 0.4296f, 0.6562f, 0.3f };        
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, lake_ambuse);

            for (int ll = 0; ll < 6; ll++)
            {
                GL.glPushMatrix();

                GL.glTranslated(160-(ll*70), 0, 0);
                GL.glColor4f(0.0117f, 0.4296f, 0.6562f, 0.5f);
                GL.glRotatef(90, 1, 0, 0);
                GLU.gluDisk(obj, 0, lake_size, 30, 30);// size of lake

                GL.glPopMatrix();
            }

        }

        public void Drawfloor()
        {
            GL.glPushMatrix();
            GL.glColor3f(0.0f, 1.0f, 0.0f);
            GL.glTranslatef(0, -0.001f, 0);
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3f(0, 1, 0);
            GL.glVertex3f(-100, 0, -100);
            GL.glVertex3f(-100, 0, 100);
            GL.glVertex3f(100, 0, 100);
            GL.glVertex3f(100, 0, -100);
            GL.glEnd();
            GL.glPopMatrix();
        }

        public void draw_sun()
        {
            GL.glPushMatrix();
            GL.glColor3d(1, 1, 1);

            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[6]);


            quad = GLU.gluNewQuadric();
            GLU.gluQuadricTexture(quad, 40);

            GL.glTranslatef(pos[0], pos[1], pos[2]);

            // rotating sun as well as all planets to Y axis
            GL.glTranslatef(0.0f, 0.0f + (float)-speed * 0.03f, 0.0f);  //y - up down
            GL.glTranslatef(0.0f, 0.0f, 0.0f + (float)-speed * 0.01f);  //z - back forward
        
            GL.glRotatef((float)moon, 0.0f, 1.0f, 0.0f);

            GLU.gluSphere(quad, 45, 1000, 1000);



            GL.glPopMatrix();


            moon += 0.04f;
        }

        public void draw_moon()
        {
            GL.glPushMatrix();

            GL.glColor3d(1, 1, 1);
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[1]);


            quad = GLU.gluNewQuadric();
            GLU.gluQuadricTexture(quad, 40);


           GL.glTranslatef(-200, 200, -300);

            // rotating moon as well as all planets to Y axis
            GL.glTranslatef(0.0f + (float)speed * 0.01f, 0.0f, 0.0f);
            GL.glTranslatef(0.0f, 0.0f + (float)speed * 0.01f, 0.0f);

            GL.glRotatef((float)moon++*0.5f, 1.0f, 1.0f, 0.0f);


            GLU.gluSphere(quad, 18, 200, 200);

            GL.glPopMatrix();

        }

        public void drawaxe()
        {

            GL.glBegin(GL.GL_LINES);
            //x  RED
            GL.glColor3f(1.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(150.0f, 0.0f, 0.0f);
            //y  GREEN 
            GL.glColor3f(0.0f, 1.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 150.0f, 0.0f);
            //z  BLUE
            GL.glColor3f(0.0f, 0.0f, 1.0f);
            GL.glVertex3f(0.0f, 0.0f, 0.0f);
            GL.glVertex3f(0.0f, 0.0f, 150.0f);
            GL.glEnd();
        }
        



        public void Draw()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);

            //TRIVIAL
            GL.glViewport(0, 0, Width, Height);
            GL.glLoadIdentity();
            GL.glEnable(GL.GL_NORMALIZE);

            GLU.gluLookAt(ScrollValue[0], ScrollValue[1], ScrollValue[2],
                           ScrollValue[3], ScrollValue[4], ScrollValue[5],
                           ScrollValue[6], ScrollValue[7], ScrollValue[8]);



            pos[0] = light_position[0] = ScrollValue[9];
            pos[1] = light_position[1] = ScrollValue[10];
            pos[2] = light_position[2] = ScrollValue[11];
            pos[3] = light_position[3] = 0;

            light_position_reflected[0] = -ScrollValue[9];
            light_position_reflected[1] = -ScrollValue[10];
            light_position_reflected[2] = -ScrollValue[11];
            light_position[3] = 0;

            GL.glLightfv(GL.GL_LIGHT0, GL.GL_POSITION, light_position);
            GL.glEnable(GL.GL_LIGHT0);

            GL.glLightfv(GL.GL_LIGHT1, GL.GL_POSITION, light_position);
            GL.glEnable(GL.GL_LIGHT1);

            //beascender look angle
            GL.glTranslatef(0.0f, -50.0f, -340.0f);//how far from the lake
            GL.glTranslatef(0.0f, 1.0f, 0.0f);//height from the lake
            GL.glRotatef(25, 1.0f, 0, 0);//look at lake angle

            GL.glRotatef(xAngle, 1.0f, 0.0f, 0.0f);
            if (checkBox)
            {
                GL.glRotatef((float)spirala * 0.018f, 0.0f, 1.0f, 0.0f);
                GL.glRotatef((float)speed * 0.008f, 1.0f, 1.0f, 0.0f);
                GL.glRotatef((float)speed * 0.008f, 0.0f, 1.0f, 1.0f);      
                GL.glRotatef((float)speed * -0.005f, 1.0f, 0.0f, 1.0f);

            }
            GL.glRotatef(yAngle, 0.0f, 1.0f, 0.0f);
            GL.glRotatef(zAngle, 0.0f, 0.0f, 1.0f);
            if (checkBox)
            {
                GL.glTranslatef(xShift, yShift + (float)-speed * 0.18f, zShift);
            }
            GL.glTranslatef(xShift, yShift, zShift);






            /*
             * 
             * Reflection drawing area start here 
             * 
             */

            GL.glPushMatrix();

            GL.glEnable(GL.GL_BLEND);
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);

            //draw only to STENCIL buffer
            GL.glEnable(GL.GL_STENCIL_TEST);
            GL.glStencilOp(GL.GL_REPLACE, GL.GL_REPLACE, GL.GL_REPLACE);
            GL.glStencilFunc(GL.GL_ALWAYS, 1, 0xFFFFFFFF);
            GL.glColorMask((byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE, (byte)GL.GL_FALSE);
            GL.glDisable(GL.GL_DEPTH_TEST);

            Drawlake();//Draw area when we want to see reflect

            // restore regular seascendings
            GL.glColorMask((byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE, (byte)GL.GL_TRUE);
            GL.glEnable(GL.GL_DEPTH_TEST);

            // reflection is drawn only where STENCIL buffer value equal to 1
            GL.glStencilFunc(GL.GL_EQUAL, 1, 0xFFFFFFFF);
            GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);


            /*
             * draw reflected scene 
             */
             
            GL.glScalef(1, -1, 1); //swap axes down 


            GL.glPushMatrix();
            DrawTexturedCube();

            //reflected penguin
            drawPenguin();

            draw_sun();
            draw_moon();
            GL.glPopMatrix();


            GL.glPopMatrix();
            GL.glEnable(GL.GL_LIGHTING);

            Drawlake();

            GL.glDisable(GL.GL_LIGHTING);


            GL.glStencilFunc(GL.GL_NOTEQUAL, 1, 0xFFFFFFFF);
            GL.glStencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);

            GL.glDepthMask((byte)GL.GL_FALSE);
            GL.glDepthMask((byte)GL.GL_TRUE);

            drawFloorTextured();

            draw_sun();
            draw_moon();


            GL.glDisable(GL.GL_STENCIL_TEST);

            DrawTexturedCube();    //SKY BOX






            /*
             * 
             * paint main scene area - start here
             * 
             */

           // drawaxe();



            GL.glShadeModel(GL.GL_FLAT);

            GL.glEnable(GL.GL_LIGHTING);

            //Main Penguin
            drawPenguin();

   

            /////////

            GL.glDisable(GL.GL_LIGHTING);








            /*
             * 
             * Draw shadows area - start here
             * 
             */
            GL.glDisable(GL.GL_LIGHTING);


            GL.glPushMatrix();

            MakeShadowMatrix(ground);  //sending fround matrix
            GL.glMultMatrixf(cubeXform);

            GL.glShadeModel(GL.GL_FLAT);
            GL.glColor3d(0, 0, 0);//black
        
            drawPenguinShade();

            GL.glPopMatrix();





            GL.glFlush();
            WGL.wglSwapBuffers(m_uint_DC);
        }


        /*
        * 
        * SHADOWS FUNCS
        * 
        */
        void ReduceToUnit(float[] vector)
        {
            float length;

            // Calculate the length of the vector		
            length = (float)Math.Sqrt((vector[0] * vector[0]) +
                                (vector[1] * vector[1]) +
                                (vector[2] * vector[2]));

            // Keep the program from blowing up by providing an exceptable
            // value for vectors that may calculated too close to zero.
            if (length == 0.0f)
                length = 1.0f;

            // Dividing each element by the length will result in a
            // unit normal vector.
            vector[0] /= length;
            vector[1] /= length;
            vector[2] /= length;
        }

        const int x = 0;
        const int y = 1;
        const int z = 2;

        // Points p1, p2, & p3 specified in counter clock-wise order
        void calcNormal(float[,] v, float[] outp)
        {
            float[] v1 = new float[3];
            float[] v2 = new float[3];

            // Calculate two vectors from the three points
            v1[x] = v[0, x] - v[1, x];
            v1[y] = v[0, y] - v[1, y];
            v1[z] = v[0, z] - v[1, z];

            v2[x] = v[1, x] - v[2, x];
            v2[y] = v[1, y] - v[2, y];
            v2[z] = v[1, z] - v[2, z];

            // Take the cross product of the two vectors to get
            // the normal vector which will be stored in out
            outp[x] = Math.Abs(v1[y] * v2[z] - v1[z] * v2[y]);
            outp[y] = Math.Abs(v1[z] * v2[x] - v1[x] * v2[z]);//Abs added..
            outp[z] = Math.Abs(v1[x] * v2[y] - v1[y] * v2[x]);

            // Normalize the vector (shorten length to one)
            ReduceToUnit(outp);
        }

        // Creates a shadow projection matrix out of the plane equation
        // coefficients and the position of the light. The return value is stored
        // in cubeXform[,]
        void MakeShadowMatrix(float[,] points)
        {
            float dot;

            // Find the plane equation coefficients
            // Find the first three coefficients the same way we
            // find a normal.
            calcNormal(points, planeCoeff);

            // Find the last coefficient by back substitutions
            planeCoeff[3] = -(
                (planeCoeff[0] * points[2, 0]) + (planeCoeff[1] * points[2, 1]) +
                (planeCoeff[2] * points[2, 2]));


            // Dot product of plane and light position
            dot = planeCoeff[0] * pos[0] +
                    planeCoeff[1] * pos[1] +
                    planeCoeff[2] * pos[2] +
                    planeCoeff[3];

            // Now do the projection
            // First column
            cubeXform[0] = dot - pos[0] * planeCoeff[0];
            cubeXform[4] = 0.0f - pos[0] * planeCoeff[1];
            cubeXform[8] = 0.0f - pos[0] * planeCoeff[2];
            cubeXform[12] = 0.0f - pos[0] * planeCoeff[3];

            // Second column
            cubeXform[1] = 0.0f - pos[1] * planeCoeff[0];
            cubeXform[5] = dot - pos[1] * planeCoeff[1];
            cubeXform[9] = 0.0f - pos[1] * planeCoeff[2];
            cubeXform[13] = 0.0f - pos[1] * planeCoeff[3];

            // Third Column
            cubeXform[2] = 0.0f - pos[2] * planeCoeff[0];
            cubeXform[6] = 0.0f - pos[2] * planeCoeff[1];
            cubeXform[10] = dot - pos[2] * planeCoeff[2];
            cubeXform[14] = 0.0f - pos[2] * planeCoeff[3];

            // Fourth Column
            cubeXform[3] = 0.0f - pos[3] * planeCoeff[0];
            cubeXform[7] = 0.0f - pos[3] * planeCoeff[1];
            cubeXform[11] = 0.0f - pos[3] * planeCoeff[2];
            cubeXform[15] = dot - pos[3] * planeCoeff[3];
        }


        protected virtual void InitializeGL()
        {
            m_uint_HWND = (uint)p.Handle.ToInt32();
            m_uint_DC = WGL.GetDC(m_uint_HWND);

            // Not doing the following WGL.wglSwapBuffers() on the DC will
            // result in a failure to subsequently create the RC.
            WGL.wglSwapBuffers(m_uint_DC);

            WGL.PIXELFORMATDESCRIPTOR pfd = new WGL.PIXELFORMATDESCRIPTOR();
            WGL.ZeroPixelDescriptor(ref pfd);
            pfd.nVersion = 1;
            pfd.dwFlags = (WGL.PFD_DRAW_TO_WINDOW | WGL.PFD_SUPPORT_OPENGL | WGL.PFD_DOUBLEBUFFER);
            pfd.iPixelType = (byte)(WGL.PFD_TYPE_RGBA);
            pfd.cColorBits = 32;
            pfd.cDepthBits = 32;
            pfd.iLayerType = (byte)(WGL.PFD_MAIN_PLANE);

            int pixelFormatIndex = 0;
            pixelFormatIndex = WGL.ChoosePixelFormat(m_uint_DC, ref pfd);
            if (pixelFormatIndex == 0)
            {
                MessageBox.Show("Unable to retrieve pixel format");
                return;
            }

            if (WGL.SetPixelFormat(m_uint_DC, pixelFormatIndex, ref pfd) == 0)
            {
                MessageBox.Show("Unable to set pixel format");
                return;
            }
            //Create rendering context
            m_uint_RC = WGL.wglCreateContext(m_uint_DC);
            if (m_uint_RC == 0)
            {
                MessageBox.Show("Unable to get rendering context");
                return;
            }
            if (WGL.wglMakeCurrent(m_uint_DC, m_uint_RC) == 0)
            {
                MessageBox.Show("Unable to make rendering context current");
                return;
            }


            initRenderinspeedL();
        }

        public void OnResize()
        {
            Width = p.Width;
            Height = p.Height;
            GL.glViewport(0, 0, Width, Height);
            Draw();
        }

        protected virtual void initRenderinspeedL()
        {
            if (m_uint_DC == 0 || m_uint_RC == 0)
                return;
            if (this.Width == 0 || this.Height == 0)
                return;
            //GL.glClearColor(0.5f, 0.9f, 1.0f, 1.0f);

            GL.glClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthFunc(GL.GL_LEQUAL);

            GL.glViewport(0, 0, this.Width, this.Height);
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();


            GL.glShadeModel(GL.GL_SMOOTH);

            GLU.gluPerspective(60, (float)Width / (float)Height, 0.45f, 1000.0f);

            GenerateTextures(1);

            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();

        }

        public uint[] Textures = new uint[8];
        public void GenerateTextures(int texture)
        {
            GL.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.glGenTextures(8, Textures);

            string[] imagesName ={ "posts.jpg","moon.jpg",
                                    "negts.jpg","negts.jpg","stars.jpg","negy.jpg","sun.jpg","sun.jpg",};
            for (int i = 0; i < 7; i++)
            {
                Bitmap image = new Bitmap(imagesName[i]);
                image.RotateFlip(RotateFlipType.RotateNoneFlipY); //Y axis in Windows is directed downwards, while in OpenGL-upwards
                System.Drawing.Imaging.BitmapData bitmapdata;
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

                bitmapdata = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[i]);
                //2D for XYZ
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                                                              0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_byte, bitmapdata.Scan0);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int)GL.GL_LINEAR);
                GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int)GL.GL_LINEAR);

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
        }

        void drawFloorTextured()
        {
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glDisable(GL.GL_BLEND);
            GL.glColor3d(1, 1, 1);
            GL.glDisable(GL.GL_LIGHTING);
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[5]);
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3f(0, 1, 0);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-300, -0.01f, 300);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(300, -0.01f, 300);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(300, -0.01f, -300);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-300, -0.01f, -300);
            GL.glEnd();
            GL.glDisable(GL.GL_TEXTURE_2D);
            GL.glEnable(GL.GL_BLEND);
            // GL.glEnable(GL.GL_LIGHTING);

        }

         void DrawTexturedCube()
        {
            GL.glEnable(GL.GL_TEXTURE_2D);
            GL.glDisable(GL.GL_BLEND);
            GL.glColor3d(1, 1, 1);
            GL.glDisable(GL.GL_LIGHTING);
            // front
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(300.0f, -0.01f, 300.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-300.0f, -0.01f, 300.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-300.0f, 300.0f, 300.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(300.0f, 300.0f, 300.0f);
            GL.glEnd();
            // back
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[0]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-300.0f, -0.01f, -300.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(300.0f, -0.01f, -300.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(300.0f, 300.0f, -300.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-300.0f, 300.0f, -300.0f);
            GL.glEnd();
            // left
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[2]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-300.0f, -0.01f, 300.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(-300.0f, -0.01f, -300.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(-300.0f, 300.0f, -300.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-300.0f, 300.0f, 300.0f);
            GL.glEnd();
            // right
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[3]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(300.0f, -0.01f, -300.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(300.0f, -0.01f, 300.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(300.0f, 300.0f, 300.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(300.0f, 300.0f, -300.0f);
            GL.glEnd();
            // top
            GL.glBindTexture(GL.GL_TEXTURE_2D, Textures[4]);
            GL.glBegin(GL.GL_QUADS);
            GL.glTexCoord2f(0.0f, 0.0f); GL.glVertex3f(-300.0f, 300.0f, -300.0f);
            GL.glTexCoord2f(1.0f, 0.0f); GL.glVertex3f(300.0f, 300.0f, -300.0f);
            GL.glTexCoord2f(1.0f, 1.0f); GL.glVertex3f(300.0f, 300.0f, 300.0f);
            GL.glTexCoord2f(0.0f, 1.0f); GL.glVertex3f(-300.0f, 300.0f, 300.0f);
            GL.glEnd();

            GL.glDisable(GL.GL_TEXTURE_2D);
            GL.glEnable(GL.GL_BLEND);
        }

     
        public void drawPenguin()
        {


            GL.glPushMatrix();
            //  drawaxe();
            GL.glRotatef(90, 0.0f, 1.0f, 0.0f);

            GL.glRotatef(-90, 1.0f, 0.0f, 0.0f);


            //drawaxe();
            GL.glTranslated(0.0, -200, 0);
            GL.glTranslated(0.0, speed, 0);  // going forward
            GL.glRotated(spirala, 0, 0, 1);    //turn 180 deg

            // drawaxe();




            if (speedFlg)
            {
                if (!spiralaFlg)
                {
                    if (forward)
                    {
                        if (speed < 380)
                        {
                            speed+=5;
                            GL.glRotated(flap * 6, 0, 0, 1);  //Jump

                        }
                        if (speed >= 380)
                        {
                            speedFlg = false;
                            spiralaFlg = true;
                            forward = false;
                        }
                    }
                    if (!forward)
                    {
                        if (speed > 0)
                        {
                            speed-=5;
                            GL.glRotated(flap*6, 0, 0, 1);

                        }
                        if (speed <= 0)
                        {
                            speedFlg = false;
                            spiralaFlg = true;
                            forward = true;
                        }
                    }
                }
              
            }
            if (!speedFlg)
            {
                if(spiralaFlg)
                {
                    if (spirala < spindLim)
                    {
                        spirala+=18;
                        GL.glTranslated(0.0, 0, 0 + flap * 8);
                    }
                    else
                    {
                        spiralaFlg = false;
                        spindLim += 180;
                    }
                }
                if (!spiralaFlg)
                {
                        speedFlg = true;

                    
                }

            }

            if (!checkBox)
            {
                if (!flapFlg)
                {
                    flap += 0.5f;
                    if (flap >= 5)
                        flapFlg = true;
                }
                if (flapFlg)
                {
                    flap -= 0.5f;
                    if (flap <=0)
                        flapFlg = false;
                }
            }
            if (checkBox)
                flap = 0;



           
            GL.glPushMatrix();

            GL.glScaled(scale_inc, scale_inc, scale_inc); //size of penguin 
             //drawaxe();

            float[] front_ambuse = { 0.9f, 0.9f, 0.9f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, front_ambuse);

            GL.glColor3f(1.0f, 1.0f, 1.0f);
            //Front body
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 3, 0);
            GL.glVertex3d(3, 3, 0);
            GL.glVertex3d(4, 4, 5);
            GL.glVertex3d(-4, 4, 5);
            GL.glEnd();

            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-4, 4, 5);
            GL.glVertex3d(4, 4, 5);
            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glEnd();

            float[] backLow_ambuse = { 0.01f, 0.01f, 0.01f, 1.0f };
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, backLow_ambuse);

            GL.glColor3f(0.01f, 0.01f, 0.01f); 

            //Back body
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 0, 0);
            GL.glVertex3d(3, 0, 0);
            GL.glVertex3d(4, -1, 5);
            GL.glVertex3d(-4, -1, 5);
            GL.glEnd();

            float[] backHigh_ambuse = { 0.18f, 0.18f, 0.18f, 1.0f };
            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, backHigh_ambuse);

            GL.glColor3f(0.18f, 0.18f, 0.18f);

            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-4, -1, 5);
            GL.glVertex3d(4, -1, 5);
            GL.glVertex3d(2.5, 0, 12);
            GL.glVertex3d(-2.5, 0, 12);
            GL.glEnd();


            float[] SideFront_ambuse = { 0.91f, 0.91f, 0.91f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, SideFront_ambuse);
            GL.glColor3f(0.91f, 0.91f, 0.91f);

            //leftSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 3, 0);
            GL.glVertex3d(-3.5, 1, 0);
            GL.glVertex3d(-4, 4, 5);
            GL.glVertex3d(-4.2, 1, 5.5);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glEnd();

            //rightSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(3, 3, 0);
            GL.glVertex3d(3.5, 1, 0);
            GL.glVertex3d(4, 4, 5);
            GL.glVertex3d(4.2, 1, 5.5);
            GL.glVertex3d(2.5, 3, 12);
            GL.glEnd();


            float[] SideBack_ambuse = { 0.1f, 0.1f, 0.1f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, SideBack_ambuse);
            GL.glColor3f(0.1f, 0.1f, 0.1f);

            //leftSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 0, 0);
            GL.glVertex3d(-3.5, 1, 0);
            GL.glVertex3d(-4, -1, 5);
            GL.glVertex3d(-4.2, 1, 5.5);
            GL.glVertex3d(-2.5, 0, 12);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glEnd();

            //rightSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(3, 0, 0);
            GL.glVertex3d(3.5, 1, 0);
            GL.glVertex3d(4, -1, 5);
            GL.glVertex3d(4.2, 1, 5.5);
            GL.glVertex3d(2.5, 0, 12);
            GL.glVertex3d(2.5, 3, 12);
            GL.glEnd();


            ////// neck
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);

            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glVertex3d(0, 1.0, 15);
            GL.glEnd();

            GL.glColor3f(0f, 0.0f, 0.0f);

            float[] beak_ambuse = { 0.6021f, 0.3001f, 0.0156f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, beak_ambuse);
            GL.glColor3f(0.6021f, 0.3001f, 0.0156f);

            ////// beak -left
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glVertex3d(0, 1.0, 15);
            GL.glVertex3d(0, 6.0, 10);
            GL.glEnd();
            ////// beak -right
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(0, 1.0, 15);
            GL.glVertex3d(0, 6.0, 10);
            GL.glEnd();


            float[] wing_ambuse = { 0.7421f, 0.4101f, 0.0156f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, wing_ambuse);
            GL.glColor3f(0.7421f, 0.4101f, 0.0156f);

            ////// wing -left
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glVertex3d(-4.2, +1.0, 5.5);
            GL.glVertex3d(-5 + (float)-flap * 0.4, -5.0 + (float)flap*0.4, 4);

            GL.glEnd();
            ////// wing -right
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(4.2, +1.0, 5.5);
            GL.glVertex3d(5 + (float)flap * 0.4, -5.0 + (float)flap * 0.4, 4);
            GL.glEnd();


            //Head
            float[] head_ambuse = { 0.1117f, 0.1296f, 0.1562f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, head_ambuse);
            GL.glColor3f(0.1117f, 0.1296f, 0.1562f);

            GL.glPushMatrix();
            GL.glTranslated(0, 1.5, 13);
            GLUT.glutSolidSphere(3.2, 64, 64);
            GL.glPopMatrix();



            //Legs
            float[] legs_ambuse = { 0.5021f, 0.2001f, 0.0156f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, legs_ambuse);
            GL.glColor3f(0.1117f, 0.1296f, 0.1562f);

            GL.glPushMatrix();
            GL.glTranslated(-2.2, 3, 0);
            GLUT.glutSolidSphere(1.8, 32, 100);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(2.2, 3, 0);
            GLUT.glutSolidSphere(1.8, 32, 100);
            GL.glPopMatrix();


            GL.glPushMatrix();





            //Eyes
            float[] eyesW_ambuse = { 1f, 1f, 1f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, eyesW_ambuse);
            GL.glColor3f(1f, 1f, 1f);

            GL.glPushMatrix();
            GL.glTranslated(-1.2, 3.5, 13);
            GLUT.glutSolidSphere(1, 32, 32);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(1.2, 3.5, 13);
            GLUT.glutSolidSphere(1, 32, 32);
            GL.glPopMatrix();

            float[] eyesB_ambuse = { 0f, 0f, 0f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, eyesB_ambuse);
            GL.glColor3f(1f, 1f, 1f);

            GL.glPushMatrix();
            GL.glTranslated(-1.2, 3.9, 12.8);
            GLUT.glutSolidSphere(0.7, 32, 32);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(1.2, 3.9, 12.8);
            GLUT.glutSolidSphere(0.7, 32, 32);
            GL.glPopMatrix();

            GL.glPopMatrix();
            GL.glPopMatrix();
            GL.glPopMatrix();

        }



        public void drawPenguinShade()
        {

            GL.glPushMatrix();
            //  drawaxe();
            GL.glRotatef(90, 0.0f, 1.0f, 0.0f);

            GL.glRotatef(-90, 1.0f, 0.0f, 0.0f);


           // drawaxe();
            GL.glTranslated(0.0, -200, 0);
            GL.glTranslated(0.0, speed, 0);
            GL.glRotated(spirala, 0, 0, 1);

            // drawaxe();




            if (speedFlg)
            {
                if (!spiralaFlg)
                {
                    if (forward)
                    {
                        if (speed < 380)
                        {
                            speed += 5;
                            GL.glRotated(flap, 0, 0, 1);

                        }
                        if (speed >= 380)
                        {
                            //speedLim += 50;
                            speedFlg = false;
                            spiralaFlg = true;
                            forward = false;
                        }
                    }
                    if (!forward)
                    {
                        if (speed > 0)
                        {
                            speed -= 5;
                            GL.glRotated(flap * 4, 0, 0, 1);

                        }
                        if (speed <= 0)
                        {
                            //speedLim += 50;
                            speedFlg = false;
                            spiralaFlg = true;
                            forward = true;
                        }
                    }
                }

            }
            if (!speedFlg)
            {
                if (spiralaFlg)
                {
                    if (spirala < spindLim)
                    {
                        spirala += 18;
                    }
                    else
                    {
                        spiralaFlg = false;
                        spindLim += 180;
                    }
                }
                if (!spiralaFlg)
                {
                    speedFlg = true;


                }

            }

            if (!checkBox)
            {
                if (!flapFlg)
                {
                    flap += 0.5f;
                    if (flap >= 5)
                        flapFlg = true;
                }
                if (flapFlg)
                {
                    flap -= 0.5f;
                    if (flap <= -5)
                        flapFlg = false;
                }
            }
            if (checkBox)
                flap = 0;

           
            GL.glPushMatrix();

            GL.glScaled(scale_inc, scale_inc, scale_inc); //size of plane 
                                                          //drawaxe();

            float[] front_ambuse = { 0.9f, 0.9f, 0.9f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, front_ambuse);

            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black
            //Front body
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 3, 0);
            GL.glVertex3d(3, 3, 0);
            GL.glVertex3d(4, 4, 5);
            GL.glVertex3d(-4, 4, 5);
            GL.glEnd();

            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-4, 4, 5);
            GL.glVertex3d(4, 4, 5);
            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glEnd();

            float[] backLow_ambuse = { 0.01f, 0.01f, 0.01f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, backLow_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            //Back body
            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 0, 0);
            GL.glVertex3d(3, 0, 0);
            GL.glVertex3d(4, -1, 5);
            GL.glVertex3d(-4, -1, 5);
            GL.glEnd();

            float[] backHigh_ambuse = { 0.18f, 0.18f, 0.18f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, backHigh_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            GL.glBegin(GL.GL_QUADS);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-4, -1, 5);
            GL.glVertex3d(4, -1, 5);
            GL.glVertex3d(2.5, 0, 12);
            GL.glVertex3d(-2.5, 0, 12);
            GL.glEnd();


            float[] SideFront_ambuse = { 0.91f, 0.91f, 0.91f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, SideFront_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            //leftSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 3, 0);
            GL.glVertex3d(-3.5, 1, 0);
            GL.glVertex3d(-4, 4, 5);
            GL.glVertex3d(-4.2, 1, 5.5);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glEnd();

            //rightSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(3, 3, 0);
            GL.glVertex3d(3.5, 1, 0);
            GL.glVertex3d(4, 4, 5);
            GL.glVertex3d(4.2, 1, 5.5);
            GL.glVertex3d(2.5, 3, 12);
            GL.glEnd();


            float[] SideBack_ambuse = { 0.1f, 0.1f, 0.1f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, SideBack_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            //leftSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-3, 0, 0);
            GL.glVertex3d(-3.5, 1, 0);
            GL.glVertex3d(-4, -1, 5);
            GL.glVertex3d(-4.2, 1, 5.5);
            GL.glVertex3d(-2.5, 0, 12);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glEnd();

            //rightSide-front body
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(3, 0, 0);
            GL.glVertex3d(3.5, 1, 0);
            GL.glVertex3d(4, -1, 5);
            GL.glVertex3d(4.2, 1, 5.5);
            GL.glVertex3d(2.5, 0, 12);
            GL.glVertex3d(2.5, 3, 12);
            GL.glEnd();


            ////// neck
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);

            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glVertex3d(0, 1.0, 15);
            GL.glEnd();

            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            float[] beak_ambuse = { 0.6021f, 0.3001f, 0.0156f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, beak_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            ////// beak -left
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glVertex3d(0, 1.0, 15);
            GL.glVertex3d(0, 6.0, 10);
            GL.glEnd();
            ////// beak -right
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(0, 1.0, 15);
            GL.glVertex3d(0, 6.0, 10);
            GL.glEnd();


            float[] wing_ambuse = { 0.7421f, 0.4101f, 0.0156f, 1.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, wing_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            ////// wing -left
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(-2.5, 3, 12);
            GL.glVertex3d(-4.2, +1.0, 5.5);
            GL.glVertex3d(-5, -5.0, 4);
            GL.glEnd();
            ////// wing -right
            GL.glBegin(GL.GL_TRIANGLE_STRIP);
            GL.glNormal3d(0, 0, 1);
            GL.glVertex3d(2.5, 3, 12);
            GL.glVertex3d(4.2, +1.0, 5.5);
            GL.glVertex3d(5, -5.0, 4);
            GL.glEnd();


            //Head
            float[] head_ambuse = { 0.1117f, 0.1296f, 0.1562f, 0.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, head_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            GL.glPushMatrix();
            GL.glTranslated(0, 1.5, 13);
            GLUT.glutSolidSphere(3.2, 64, 64);
            GL.glPopMatrix();



            //Legs
            float[] legs_ambuse = { 0.5021f, 0.2001f, 0.0156f, 0.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, legs_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            GL.glPushMatrix();
            GL.glTranslated(-2.2, 3, 0);
            GLUT.glutSolidSphere(1.8, 32, 100);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(2.2, 3, 0);
            GLUT.glutSolidSphere(1.8, 32, 100);
            GL.glPopMatrix();


            GL.glPushMatrix();





            //Eyes
            float[] eyesW_ambuse = { 1f, 1f, 1f, 0.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, eyesW_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            GL.glPushMatrix();
            GL.glTranslated(-1.2, 3.5, 13);
            GLUT.glutSolidSphere(1, 32, 32);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(1.2, 3.5, 13);
            GLUT.glutSolidSphere(1, 32, 32);
            GL.glPopMatrix();

            float[] eyesB_ambuse = { 0f, 0f, 0f, 0.0f };

            GL.glMaterialfv(GL.GL_FRONT_AND_BACK, GL.GL_AMBIENT_AND_DIFFUSE, eyesB_ambuse);
            GL.glColor3f(0.0f, 0.0f, 0.0f);  //black

            GL.glPushMatrix();
            GL.glTranslated(-1.2, 3.9, 12.8);
            GLUT.glutSolidSphere(0.7, 32, 32);
            GL.glPopMatrix();
            GL.glPushMatrix();
            GL.glTranslated(1.2, 3.9, 12.8);
            GLUT.glutSolidSphere(0.7, 32, 32);
            GL.glPopMatrix();

            GL.glPopMatrix();
            GL.glPopMatrix();
            GL.glPopMatrix();
        }

    }



}


