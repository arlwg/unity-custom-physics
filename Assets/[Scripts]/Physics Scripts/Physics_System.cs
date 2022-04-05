using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Physics_System : MonoBehaviour
{


    public Vector3 gravity = new Vector3(0, -9.81f, 0);
    private bool _gravityenabled = true;
    public float globalGravityScale = 1;
    public List<Physics_Object> physicsObjects = new List<Physics_Object>();
    [SerializeField]
    public Slider GravitySlider;
    [SerializeField]
    public Toggle GravityToggle;
    public 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        globalGravityScale = GravitySlider.value;
        _gravityenabled = GravityToggle;
        //Velocity
        foreach(Physics_Object obj in physicsObjects)
        {
            if (!obj.lockPosition)
            {
                //If the object isn't colliding, accelerate it by gravity (Free-Fall)
                if (!obj.gravityLock && _gravityenabled)
                {
                    obj.velocity += (gravity * (obj.gravityScale * globalGravityScale * Time.deltaTime)) ;
                }
                
                    
            }
        }
        //Position
        foreach(Physics_Object obj in physicsObjects)
        {
            //If position isn't locked, transform position by the objects velocity.
            if (!obj.lockPosition)
            {
                
                obj.transform.position = obj.transform.position +  obj.velocity * Time.fixedDeltaTime;
            }
        }

       CollisionUpdate();
    }

    bool EnableGravity()
    {
        return !_gravityenabled;
    }

     void getLocked(Physics_Object a, Physics_Object b, out float movementScalarA, out float movementScalarB)
    {
        
        movementScalarA = 0.5f;
        movementScalarB = 0.5f;

        //Cases for different lock positions.
        if (a.lockPosition && b.lockPosition)
        {
            movementScalarB = 0;
            movementScalarA = 0;
        }
        if (a.lockPosition && !b.lockPosition)
        {
            movementScalarB = 1.0f;
            movementScalarA = 0;
        }
        if (!a.lockPosition && b.lockPosition)
        {
            movementScalarB = 0;
            movementScalarA = 1.0f;
        }
        if (!a.lockPosition && !b.lockPosition)
        {
            movementScalarB = 0.5f;
            movementScalarA = 0.5f;
        }

    }

    void SphereSphereCollision(Physics_ColliderSphere a, Physics_ColliderSphere b)
    {

        //Distance between the sphere centers.
        Vector3 displacement = b.transform.position - a.transform.position;

        //Get the magnitude of the displacement between the spheres
        float distance = displacement.magnitude;

        //The total radius of both spheres.
        float sumRadii = a.radius + b.radius;

        //Get the difference between the radius of the two spheres and the distance between the origins.
        float penetrationDepth = sumRadii - distance;

        // If distance is not greater than the sum of the two spheres, we are colliding.
        bool isOverlapping = penetrationDepth > 0;


        //Collision response.
        if (isOverlapping)
        {
            Debug.Log(a.name + ("Collided with : ") + b.name);

            Color colorA =  a.GetComponent<Renderer>().material.color;
            Color colorB =  b.GetComponent<Renderer>().material.color;
            
            a.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            b.GetComponent<Renderer>().material.color = Color.Lerp(colorB, colorA, 0.05f);
           

        }
        else
        {
            return;
        }


        //Collision Normal
        Vector3 collisionNormalAtoB;

        //Minimum distance that is required to trigger a collision response.
        float minDistance = 0.0001f;
        
        //If the magnitude of the distance is greater than 0.0001 set the collision normal to the y axis with the penetration depth.
        if (distance < minDistance)
        {
            collisionNormalAtoB = new Vector3(0, penetrationDepth, 0);
        }
        else
        {
            //Reset the collision normal
            collisionNormalAtoB = displacement / distance;
        }

        //Are the objects locked? Affects the movement scalars.
        getLocked(a.kinematicsObject, b.kinematicsObject, out float movementScalarA, out float movementScalarB);

        Vector3 minimumTranslationVectorAtoB = penetrationDepth * collisionNormalAtoB;
        Vector3 ContactPoint = a.transform.position + collisionNormalAtoB * a.radius;

        //apply minimum translation, call other functions such as elasticity and friction.
        ApplyMinimumTranslationVector(a.kinematicsObject, b.kinematicsObject, minimumTranslationVectorAtoB, ContactPoint, collisionNormalAtoB);
       

    }

    

    void SpherePlaneCollision(Physics_ColliderSphere a, Physics_ColliderPlane b)
    {

       
        Vector3 PointOnPlane = b.transform.position;

        Vector3 SphereCentre = a.transform.position;

        //Vector from point on the plane to the center of the sphere
        Vector3 displacement = SphereCentre - PointOnPlane;
        
       
       
        //Dot product of the plane-sphere vector and the normal of the plane.
        //If this dot is negative, the sphere is behind the plane. If it is positive, the sphere center is in front of the plane.
        float dot = Vector3.Dot(displacement, b.GetNormal());


        //Represents distance from plane to center of the sphere.
        float distance = Mathf.Abs(dot);

        //Get the depth of the penetration depending on the dot product of the normal(direction of the plane) and the displacement of the shapes inside each other.
        float penetrationDepth = distance - a.radius + 0.001f;

        //Get the penetration direction vector based upon the normal.
        Vector3 Penetration = b.GetNormal() * penetrationDepth;



        //If the distance from the plane to the center of the sphere is less than the radius of the sphere, they are overlapping.
        bool isOverlapping = distance <= a.radius;

       //If shapes are overlapping currently, do something. ( Collision response )
        if (isOverlapping)  
        {
            Debug.Log(a.name + ("Collided with : ") + b.name);

            Color colorA = a.GetComponent<Renderer>().material.color;
            Color colorB = b.GetComponent<Renderer>().material.color;

            //Collision Indication - changes colors
            a.GetComponent<Renderer>().material.color = Color.Lerp(colorA, colorB, 0.05f);
            b.GetComponent<Renderer>().material.color = Color.Lerp(colorB, colorA, 0.05f);
            //a.kinematicsObject.velocity *= -0.5f;
            a.kinematicsObject.gravityLock = true;


        }
        else
        {
            //Turn the gravity acceleration back on when the sphere is not colliding.
            a.kinematicsObject.gravityLock = false;
            return;
        }


        //---------------IF IS STILL OVERLAPPING--------------------------

        //get whether the objects are locked or not.
        getLocked(a.kinematicsObject, b.kinematicsObject, out float movementScalarA, out float movementScalarB);

        Vector3 ContactPoint = a.transform.position + b.GetNormal() * a.radius;


        //apply minimum translation, call other functions such as elasticity and friction.
        ApplyMinimumTranslationVector(a.kinematicsObject, b.kinematicsObject, Penetration, ContactPoint, b.GetNormal());
    }


    
    void AABBAABBCollision(Physics_ColliderAABB objectAShape, Physics_ColliderAABB objectBShape)
    {

        Vector3 halfSizeA = objectAShape.GetHalfSize();
        Vector3 halfSizeB = objectBShape.GetHalfSize();
        // Get displacement between the boxes
        Vector3 displacementAB = objectBShape.transform.position - objectAShape.transform.position;
        // Find the length of these projections along each axis
        // Compare the distance between them along each axis with half of their siSizeze.x - displacementAB.x)) in that axis
         float penetrationX = (halfSizeA.x + halfSizeB.x - Mathf.Abs(displacementAB.x));
         float penetrationY = (halfSizeA.y + halfSizeB.y - Mathf.Abs(displacementAB.y));
         float penetrationZ = (halfSizeA.z + halfSizeB.z - Mathf.Abs(displacementAB.z));
         // IF the distance (along that axis) is less than the sum of their half-sizes ( along that axis) then they must be overlapping
         if (penetrationX < 0 || penetrationY < 0 || penetrationZ < 0)
         {
             return; // no collision
         }
         
         // The minimum translation neccessary to push the object back.
         Vector3 normal = new Vector3(Mathf.Sign(displacementAB.x), 0, 0);
         Vector3 minimumTranslationVector = normal * penetrationX;;
        
       
         // Find the shortest penetration to move along
         if (penetrationX < penetrationY && penetrationX < penetrationZ) // is penX the shortest?
         {
             normal = new Vector3(Mathf.Sign(displacementAB.x),0, 0);
             minimumTranslationVector = normal * penetrationX;
         }
         else if (penetrationY < penetrationX && penetrationY < penetrationZ) // is penY the shortest?
         {
             normal = new Vector3(0,Mathf.Sign(displacementAB.y), 0);
             minimumTranslationVector = normal * penetrationY;
         }
         else //if (penetrationZ < penetrationY && penetrationZ < penetrationX) // is penZ the shortest?   // could just be else
         {
             normal = new Vector3(0,0, Mathf.Sign(displacementAB.z));
             minimumTranslationVector = normal * penetrationZ;
         }

         Vector3 contactPoint = objectAShape.transform.position + minimumTranslationVector;

         // Find the minimum translation vector to move them
         // Apply displacement to separate them along the shortest path we can
         // Apply minimum translation, call other functions such as elasticity and friction.
         ApplyMinimumTranslationVector(objectAShape.kinematicsObject, objectBShape.kinematicsObject, minimumTranslationVector, contactPoint, normal);

    }


    void SphereAABBCollision(Physics_ColliderSphere a, Physics_ColliderAABB b)
    {


        Vector3 displacement = b.transform.position - a.transform.position;
        Vector3 penetration = new Vector3();
        penetration.x = (b.GetHalfSize().x + a.radius - Mathf.Abs(displacement.x));
        penetration.y = (b.GetHalfSize().y + a.radius - Mathf.Abs(displacement.y));
        penetration.z = (b.GetHalfSize().z + a.radius - Mathf.Abs(displacement.z));

        if (penetration.x < 0 || penetration.y < 0 || penetration.z < 0)
        {
            return; // No collision
        }

        Vector3 normal = new Vector3();
        Vector3 minTranslationVectorForA = new Vector3();

        if (penetration.x <= penetration.y && penetration.x <= penetration.z)
        {
            normal = new Vector3(Mathf.Sign(displacement.x), 0, 0);
            minTranslationVectorForA = normal * penetration.x;
        }
        else if (penetration.y <= penetration.x && penetration.y <= penetration.z)
        {
            normal = new Vector3(0, Mathf.Sign(displacement.y), 0);
            minTranslationVectorForA = normal * penetration.y;
        }
        else if (penetration.z <= penetration.x && penetration.z <= penetration.y)
        {
            normal = new Vector3(0, 0, Mathf.Sign(displacement.z));
            minTranslationVectorForA = normal * penetration.z;
        }

        Vector3 contactPoint = a.transform.position + minTranslationVectorForA;
        ApplyMinimumTranslationVector(a.kinematicsObject, b.kinematicsObject, minTranslationVectorForA, contactPoint, normal);
    }

    
    void ApplyMinimumTranslationVector(Physics_Object a, Physics_Object b, Vector3 minimumTranslationVectorForA, Vector3 contactPoint, Vector3 normal)
    {
        ////---------------IF IS STILL OVERLAPPING-------------------------
        // Get whether the objects are locked or not.
        getLocked(a, b, out float movementScalarA, out float movementScalarB);
    

        // Calculation of the translation vectors based on the movement scalars according to whether an object is locked or not.
        // If both objects are not locked, they both move a half space, if one is lock and the other is not, the unlocked object will move a full space to compensate.
        Vector3 TranslationVectorA = -minimumTranslationVectorForA * movementScalarA;
        Vector3 TranslationVectorB = minimumTranslationVectorForA * movementScalarB;

        // Transform the position of the objects based on the translation vectors.
        a.transform.position += TranslationVectorA;
        b.transform.position += TranslationVectorB;



        CollisionInfo collisionInfo;
        collisionInfo.objectA = a.shape;
        collisionInfo.objectB = b.shape;
        collisionInfo.collisionNormalAtoB = normal;
        collisionInfo.contactPoint = contactPoint;

        ApplyCollisionResponse(collisionInfo);
    }
    void ApplyCollisionResponse(CollisionInfo collision)
    {

        Physics_Object objectA = collision.objectA.kinematicsObject;
        Physics_Object objectB = collision.objectB.kinematicsObject; 


        //Find relative velocity between objects along the normal

        Vector3 relativeVelocityAtoB = objectB.velocity - objectA.velocity;

        float relativeNormalVelocityAtoB = Vector3.Dot(relativeVelocityAtoB, collision.collisionNormalAtoB);
        //Determine coefficient of restitution
        float restitution = 0.5f * (objectA.bounciness + objectB.bounciness);
        float changeInVelocity = -relativeNormalVelocityAtoB * (1.0f + restitution);







        //Handle different cases based on which objects are locked
        //Determine impulse (Force * time) = kg * m/sec^2 * sec
        //Apply the impulse to each object.
        if (objectB.lockPosition && !objectA.lockPosition)
        {
            float impulse = changeInVelocity * objectA.mass; //??
            objectA.velocity -= collision.collisionNormalAtoB * (impulse / objectA.mass);
           
        }
        else if (!objectB.lockPosition && objectA.lockPosition)
        {
            float impulse = changeInVelocity * objectB.mass; //??
            objectB.velocity += collision.collisionNormalAtoB * (impulse / objectB.mass);


        }
        else if (!objectB.lockPosition && !objectA.lockPosition)
        {
            float impulse = changeInVelocity / (1.0f / objectB.mass + 1.0f/objectA.mass); //??

            objectA.velocity -= collision.collisionNormalAtoB * (impulse / objectA.mass);
            objectB.velocity += collision.collisionNormalAtoB * (impulse / objectB.mass);
        } 
        Vector3 relativeSurfaceVelocity = relativeVelocityAtoB - (relativeNormalVelocityAtoB * collision.collisionNormalAtoB);

       ApplyFriction(collision, relativeSurfaceVelocity);
    }



    void CollisionUpdate()
    {
        //Go through all the physicsObjects
        for(int i = 0; i < physicsObjects.Count; i++)
        {
            //Go through all the physicsObjects, adjacent to the previous loop.
            for(int j = i + 1; j < physicsObjects.Count; j++)
            {
                Physics_Object objectA = physicsObjects[i];
                Physics_Object objectB = physicsObjects[j];


                
                //If null, skip over loop.
                if(objectA.shape == null && objectB.shape == null)
                {
                    continue;
                }
                if(objectA.shape.GetCollisionShape() == CollisionShape.Sphere &&
                   objectB.shape.GetCollisionShape() == CollisionShape.AABB)
                {
                    //if the distance between spheres is less than the sum of their radii, then they are overlapping.

                    SphereAABBCollision((Physics_ColliderSphere)objectA.shape, (Physics_ColliderAABB)objectB.shape);
                }
                if(objectA.shape.GetCollisionShape() == CollisionShape.AABB &&
                   objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    //if the distance between spheres is less than the sum of their radii, then they are overlapping.

                    SphereAABBCollision((Physics_ColliderSphere)objectB.shape, (Physics_ColliderAABB)objectA.shape);
                }

                if(objectA.shape.GetCollisionShape() == CollisionShape.AABB &&
                   objectB.shape.GetCollisionShape() == CollisionShape.AABB)
                {
                    //if the distance between spheres is less than the sum of their radii, then they are overlapping.

                    AABBAABBCollision((Physics_ColliderAABB)objectA.shape, (Physics_ColliderAABB)objectB.shape);
                }

                //If both are spheres ... do sphere-sphere collision

                if(objectA.shape.GetCollisionShape() == CollisionShape.Sphere &&
                   objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    //if the distance between spheres is less than the sum of their radii, then they are overlapping.

                    SphereSphereCollision((Physics_ColliderSphere)objectA.shape, (Physics_ColliderSphere)objectB.shape);
                }

                //If sphere and plane.. Do sphere-plane collision
                if (objectA.shape.GetCollisionShape() == CollisionShape.Sphere &&
                    objectB.shape.GetCollisionShape() == CollisionShape.Plane)
                {
                    SpherePlaneCollision((Physics_ColliderSphere)objectA.shape, (Physics_ColliderPlane)objectB.shape);
                }

                if (objectA.shape.GetCollisionShape() == CollisionShape.Plane &&
                    objectB.shape.GetCollisionShape() == CollisionShape.Sphere)
                {
                    SpherePlaneCollision((Physics_ColliderSphere)objectB.shape, (Physics_ColliderPlane)objectA.shape);
                }

            }
        }
    }


    
    void ApplyFriction(CollisionInfo collision, Vector3 relativeSurfaceVelocity)
    {
        // Two objects
        Physics_Object a = collision.objectA.kinematicsObject;
        Physics_Object b = collision.objectB.kinematicsObject;

        float relativeSpeed = relativeSurfaceVelocity.magnitude;
        float minSpeed = 0.0001f;
        if (relativeSpeed < minSpeed)
        {
            return;
        }
        

        // Find coefficient of friction
        float kFrictionCoefficient = 0.5f * (a.friction + b.friction);

        // Normal
        // Force along the normal
        // Relative velocity along the common surfaces.
        // Magnitude of frictional foce is coefficient of friction times the normal force
        Vector3 directionofFriction = relativeSurfaceVelocity / relativeSpeed;

        float gravityDotNormal;
        gravityDotNormal = -Vector3.Dot(this.gravity, collision.collisionNormalAtoB);



        Vector3 accelerationFriction = -(gravityDotNormal * kFrictionCoefficient * directionofFriction);
        // Apply frictional force to objects ( take into account "lock position" ) 

        if (!a.lockPosition)
        {
            a.velocity += accelerationFriction * Time.fixedDeltaTime;
        }

        if (!b.lockPosition)
        {
            b.velocity += accelerationFriction * Time.fixedDeltaTime;
        }
       
    }
}


