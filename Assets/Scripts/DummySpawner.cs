using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class DummySpawner : MonoBehaviour
{
    public int copies;
    public GameObject dummy;
    public List<GameObject> dummies;

    public bool GPU;
    public struct AABB
    {
        public Vector3 min;
        public Vector3 max;
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
            dummies.Add(Instantiate(dummy, new Vector3(Random.Range(-50f, 50f), 1f, Random.Range(-50f, 50f)), Quaternion.identity));
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
                    if (ColliderPhyiscs.Collide(dummies[i], dummies[j]))
                    {
                        Debug.Log($"Collision between: {i} and {j}");

                        //Feedback
                        {
                            if (dummies[i].TryGetComponent(out DummyMovement dmI))
                            {
                                Vector3 poc = ColliderPhyiscs.PointOfContact(dummies[j].transform.position, dummies[i]);

                                dmI.hMove = (dummies[i].transform.position.x - poc.x) < 0 ? -dmI.RandomSpeed() : dmI.RandomSpeed();
                                dmI.vMove = (dummies[i].transform.position.z - poc.z) < 0 ? -dmI.RandomSpeed() : dmI.RandomSpeed();

                                //rbI.velocity = new Vector3(
                                //    dummies[i].transform.position.x - poc.x,
                                //    rbI.velocity.y,
                                //    dummies[i].transform.position.z- poc.z);

                                //rbI.velocity = new Vector3(
                                //    dummies[j].transform.position.x - dummies[i].transform.position.x,
                                //    rbI.velocity.y,
                                //    dummies[j].transform.position.z - dummies[i].transform.position.z);

                            }

                            if (dummies[j].TryGetComponent(out DummyMovement dmJ))
                            {
                                Vector3 poc = ColliderPhyiscs.PointOfContact(dummies[i].transform.position, dummies[j]);

                                dmJ.hMove = (dummies[j].transform.position.x - poc.x) < 0 ? -dmJ.RandomSpeed() : dmJ.RandomSpeed();
                                dmJ.vMove = (dummies[j].transform.position.z - poc.z) < 0 ? -dmJ.RandomSpeed() : dmJ.RandomSpeed();

                                //rbJ.velocity = new Vector3(
                                //    dummies[j].transform.position.x - poc.x,
                                //    rbJ.velocity.y,
                                //     dummies[j].transform.position.z- poc.z);



                                //rbJ.velocity = new Vector3(
                                //    dummies[i].transform.position.x - dummies[j].transform.position.x,
                                //    rbJ.velocity.y,
                                //    dummies[i].transform.position.z - dummies[j].transform.position.z);

                            }

                            if (dummies[i].TryGetComponent(out Renderer rndI))
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

                            if (dummies[j].TryGetComponent(out Renderer rndJ))
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
            var aabbList = new NativeArray<AABB>(dummies.Count, Allocator.Persistent);
            var colidedList = new NativeArray<CollidedWith>(dummies.Count, Allocator.Persistent);

            for (var i = 0; i < aabbList.Length; i++)
            {
                aabbList[i] = ColliderPhyiscs.Box(dummies[i]);
            }

            var job = new VelocityJob()
            {
                Colliders = aabbList,
                Collided = colidedList
            };

            JobHandle jobHandle = job.Schedule(dummies.Count, 64);

            jobHandle.Complete();

            for (var i = 0; i < aabbList.Length; i++)
            {
                if (job.Collided[i].Collided)
                {
                    //Debug.Log($"Collision between: {i} and {job.Colided[i].With}");

                    if (dummies[i].TryGetComponent(out DummyMovement dmI))
                    {
                        Vector3 poc = ColliderPhyiscs.PointOfContact(dummies[job.Collided[i].With].transform.position, dummies[i]);

                        dmI.hMove = (dummies[i].transform.position.x - poc.x) < 0 ? -dmI.RandomSpeed() : dmI.RandomSpeed();
                        dmI.vMove = (dummies[i].transform.position.z - poc.z) < 0 ? -dmI.RandomSpeed() : dmI.RandomSpeed();
                    }

                    if (dummies[i].TryGetComponent(out Renderer rndI))
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

            aabbList.Dispose();
            colidedList.Dispose();
        }
    }


    struct VelocityJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<AABB> Colliders;

        public NativeArray<CollidedWith> Collided;
        public void Execute(int i)
        {
            //for(int j = 0; j < Colliders.Length && j != i ; j++)
            for (int j = i - 1; j >= 0; j--)
            {
                if (ColliderPhyiscs.Intersect(Colliders[i], Colliders[j]))
                {
                    Collided[i] = new CollidedWith {Collided = true, With = j };
                }
            }
        }
    }

}
