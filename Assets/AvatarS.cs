using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using LightBuzz.Vitruvius;
using LightBuzz.Vitruvius.Avateering;
using Microsoft.Kinect.Face;
using System.Linq;

public class AvatarS : MonoBehaviour
{

    //Kinect
    BodyFrameReader bodyReader;
    Body[] users;
    KinectSensor sensor;

    // 3D models
    Model[] models;

    public FBX unitychan;
    //public FBX male;

    public Transform bat;
    public float scaleFactor = 1;
    //public JointType lockOnJoint = JointType.WristRight;

    // Determines whether
    // one player will control all avatars, or
    // whether each player will control one avatar
    public bool seperatedPlayers;
    // Use this for initialization

    void Start()
    {
        // 1. Initialize Kinect
        sensor = KinectSensor.GetDefault();

        if (sensor != null)
        {
            bodyReader = sensor.BodyFrameSource.OpenReader();

            sensor.Open();
        }

        // 2. Enable Avateering
        Avateering.Enable();

        // 3. Specify the 3D models to animate.
        models = new Model[]
        {
            unitychan,
            //male
        };

        // 4. Initialize each 3D model.
        for (int i = 0; i < models.Length; i++)
        {
            models[i].Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bodyReader != null)
        {

            using (var frame = bodyReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    users = frame.Bodies().
                            Where(b => b.IsTracked).ToArray();

                    if (seperatedPlayers)
                    {
                        // Each user controls a different avatar.
                        for (int index = 0; index < users.Length; index++)
                        {
                            Model model = models[index];
                            Body user = users[index];

                            // Yes, this line does ALL of the hard work for you.
                            Avateering.Update(model, user);
                        }
                    }
                    else if (users.Length > 0)
                    {
                        // A single user controls all of the avatars.
                        for (int index = 0; index < models.Length; index++)
                        {
                            Model model = models[index];
                            Body user = users[0];

                            // Yes, this line does ALL of the hard work for you.
                            Avateering.Update(model, user);

                            bat.gameObject.SetActive(true);
                            Windows.Kinect.Joint lockOnJoint = user.Joints[JointType.HandRight];
                            Vector3 maHand = lockOnJoint.Position.ToVector3();
                            bat.position = maHand;
                        }
                    }

                    /*bat.gameObject.SetActive(true);

                    BodyWrapper userchan = new BodyWrapper();
                    userchan.Set(users[0]);
                    Vector2 position2D = userchan.Map2D[lockOnJoint].ToVector2();

                    //frameView.
                  PositionOnFrame(ref position2D);
                    bat.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor) / body.Joints[lockOnJoint].Position.Z;
                    bat.position = new Vector3(position2D.x, position2D.y + basketball.localScale.y, 0);*/
                }
            }
        }
    }

    public void Dispose()
    {
        // 1. Dispose Kinect
        if (bodyReader != null)
        {
            bodyReader.Dispose();
            bodyReader = null;
        }

        if (sensor != null)
        {
            if (sensor.IsOpen)
            {
                sensor.Close();
            }

            sensor = null;
        }

        // 2. Dispose 3D models
        for (int i = 0; i < models.Length; i++)
        {
            models[i].Dispose();
        }

        // 3. Disable avateering
        Avateering.Disable();
    }
}
