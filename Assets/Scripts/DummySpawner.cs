using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    public int copies;
    public GameObject dummy;
    public GameObject dummySphere;

    public List<GameObjectShape> dummies = new List<GameObjectShape>();

    public bool GPU;
    public bool cubesOnly;
    public bool gravityTest;
    [System.Serializable]
    public struct GameObjectShape
    {
        public GameObject go;
        public bool isBox;
    }
    public struct Shape
    {
        public bool isBox;
        public AABB box;
        public Sphere sphere;
    }
    public struct AABB
    {
        public Vector3 min;
        public Vector3 max;
    }

    public struct Sphere
    {
        public Vector3 position;
        public float radius;
    }

    public struct CollidedWith
    {
        public bool Collided;
        public int With;
    }

    private void Start()
    {
        for (int i = 0; i < copies; i++)
        {
            int min = cubesOnly ? 1 : 0;

            float spawnY = 1f;
            if (gravityTest)
            {
               spawnY = Random.Range(50f, 100f);
            }
            GameObjectShape toSpawn = Random.Range(min, 2) == 0 ? new GameObjectShape { isBox = false, go = dummySphere } : new GameObjectShape { isBox = true, go = dummy };
            dummies.Add(new GameObjectShape { isBox = toSpawn.isBox, go = Instantiate(toSpawn.go, new Vector3(Random.Range(-50f, 50f), spawnY, Random.Range(-50f, 50f)), Quaternion.identity, transform) });
        }

    }

    private void Update()
    {
        if (!GPU)
        {
            for (int i = 0; i < dummies.Count - 1; i++)
            {
                for (int j = i + 1; j < dummies.Count; j++)
                {
                    Shape first = new Shape();
                    Shape second = new Shape();

                    if (dummies[i].isBox)
                    {
                        first.isBox = true;
                        first.box = ColliderPhyiscs.Box(dummies[i].go);
                    }
                    else
                    {
                        first.isBox = false;
                        first.sphere = ColliderPhyiscs.Sphere(dummies[i].go);
                    }

                    if (dummies[j].isBox)
                    {
                        second.isBox = true;
                        second.box = ColliderPhyiscs.Box(dummies[j].go);
                    }
                    else
                    {
                        second.isBox = false;
                        second.sphere = ColliderPhyiscs.Sphere(dummies[j].go);
                    }

                    if (ColliderPhyiscs.Collide(first, second))
                    {
                        Debug.Log($"Collision between: {i} and {j}");

                        //Feedback
                        {
                            if (dummies[i].go.TryGetComponent(out DummyMovement dmI))
                            {
                                Vector3 poc = ColliderPhyiscs.PointOfContact(dummies[j].go.transform.position, dummies[i].go);

                                dmI.hMove = (dummies[i].go.transform.position.x - poc.x) < 0 ? -dmI.ObjectSpeed() : dmI.ObjectSpeed();
                                dmI.vMove = (dummies[i].go.transform.position.z - poc.z) < 0 ? -dmI.ObjectSpeed() : dmI.ObjectSpeed();

                                //rbI.velocity = new Vector3(
                                //    dummies[i].transform.position.x - poc.x,
                                //    rbI.velocity.y,
                                //    dummies[i].transform.position.z- poc.z);

                                //rbI.velocity = new Vector3(
                                //    dummies[j].transform.position.x - dummies[i].transform.position.x,
                                //    rbI.velocity.y,
                                //    dummies[j].transform.position.z - dummies[i].transform.position.z);

                            }

                            if (dummies[j].go.TryGetComponent(out DummyMovement dmJ))
                            {
                                Vector3 poc = ColliderPhyiscs.PointOfContact(dummies[i].go.transform.position, dummies[j].go);

                                dmJ.hMove = (dummies[j].go.transform.position.x - poc.x) < 0 ? -dmJ.ObjectSpeed() : dmJ.ObjectSpeed();
                                dmJ.vMove = (dummies[j].go.transform.position.z - poc.z) < 0 ? -dmJ.ObjectSpeed() : dmJ.ObjectSpeed();

                                //rbJ.velocity = new Vector3(
                                //    dummies[j].transform.position.x - poc.x,
                                //    rbJ.velocity.y,
                                //     dummies[j].transform.position.z- poc.z);



                                //rbJ.velocity = new Vector3(
                                //    dummies[i].transform.position.x - dummies[j].transform.position.x,
                                //    rbJ.velocity.y,
                                //    dummies[i].transform.position.z - dummies[j].transform.position.z);

                            }

                            if (dummies[i].go.TryGetComponent(out Renderer rndI))
                            {
                                if (rndI.material.color == Color.green)
                                {
                                    rndI.material.color = Color.red;
                                }
                                else
                                {
                                    rndI.material.color = Color.green;
                                }
                            }

                            if (dummies[j].go.TryGetComponent(out Renderer rndJ))
                            {
                                if (rndJ.material.color == Color.green)
                                {
                                    rndJ.material.color = Color.red;
                                }
                                else
                                {
                                    rndJ.material.color = Color.green;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
        {
            NativeArray<CollidedWith> colidedList = new NativeArray<CollidedWith>(dummies.Count, Allocator.Persistent);
            NativeArray<Shape> shapeList = new NativeArray<Shape>(dummies.Count, Allocator.Persistent);

            for (var i = 0; i < shapeList.Length; i++)
            {
                if (dummies[i].isBox)
                {
                    shapeList[i] = new Shape { isBox = true, box = ColliderPhyiscs.Box(dummies[i].go) };
                }
                else
                {
                    shapeList[i] = new Shape { isBox = false, sphere = ColliderPhyiscs.Sphere(dummies[i].go) };
                }
            }

            var job = new VelocityJob()
            {
                Colliders = shapeList,
                Collided = colidedList
            };

            JobHandle jobHandle = job.Schedule(dummies.Count, 32);

            jobHandle.Complete();

            for (var i = 0; i < shapeList.Length; i++)
            {
                if (job.Collided[i].Collided)
                {
                    //Debug.Log($"Collision between: {i} and {job.Collided[i].With}");

                    if (dummies[i].go.TryGetComponent(out DummyMovement dmI))
                    {
                        Vector3 poc = ColliderPhyiscs.PointOfContact(dummies[job.Collided[i].With].go.transform.position, dummies[i].go);

                        dmI.hMove = (dummies[i].go.transform.position.x - poc.x) < 0 ? -dmI.ObjectSpeed() : dmI.ObjectSpeed();
                        dmI.vMove = (dummies[i].go.transform.position.z - poc.z) < 0 ? -dmI.ObjectSpeed() : dmI.ObjectSpeed();
                    }

                    if (dummies[i].go.TryGetComponent(out Renderer rndI))
                    {
                        if (rndI.material.color == Color.green)
                        {
                            rndI.material.color = Color.red;
                        }
                        else
                        {
                            rndI.material.color = Color.green;
                        }
                    }
                }
            }

            shapeList.Dispose();
            colidedList.Dispose();
        }
    }


    struct VelocityJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Shape> Colliders;

        public NativeArray<CollidedWith> Collided;
        public void Execute(int i)
        {
            for (int j = 0; j < Colliders.Length && j != i; j++)
            //for (int j = i - 1; j >= 0; j--)
            {
                if (ColliderPhyiscs.Collide(Colliders[i], Colliders[j]))
                {
                    Collided[i] = new CollidedWith { Collided = true, With = j };
                }
            }
        }
    }

}
