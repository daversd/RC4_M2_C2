using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CubeAgent : Agent
{
    #region Fields and properties
    
    //07 The environment the agent exists in
    EnvironmentManager _environment;
    //08 The rigidbody of the agent
    Rigidbody _rBody;

    //09 The component the agnet is carrying
    GameObject _holding;

    //10 Multiplier for the agent's rotation
    float _rotateFactor = 20f;
    //11 Multiplier for the agent's movement
    float _moveFactor = 50f;

    #endregion

    #region Unity standard methods

    //01 Add Start method
    private void Start()
    {
        //12 Get the environment from parent
        _environment = transform.parent.transform.GetComponent<EnvironmentManager>();
        //13 Get the agent's rigidbody
        _rBody = transform.GetComponent<Rigidbody>();
    }

    //02 Add Update method
    private void Update()
    {
        // End episode with key
        if (Input.GetKeyDown(KeyCode.R))
        {
            EndEpisode();
        }

        //78 Implement component rotation on Update 
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    _holding.transform.localEulerAngles +=  new Vector3(90, 0, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    _holding.transform.localEulerAngles +=  new Vector3(-90, 0, 0);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    _holding.transform.localEulerAngles +=  new Vector3(0, 90, 0);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    _holding.transform.localEulerAngles += new Vector3(0, -90, 0);
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    _holding.transform.localEulerAngles += new Vector3(0, 0, 90);
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    _holding.transform.localEulerAngles +=  new Vector3(0, 0, 90);
        //}
    }

    #endregion

    //03 Add OnEpisodeBegin
    public override void OnEpisodeBegin()
    {
        //14 Reset the environment
        _environment.ResetEnvironment();
        
        //15 Set holding to null
        _holding = null;

        //16 Generate a new assembly pair
        _environment.GenerateAssemblyPair();

        //17 Zero the movement of the agent
        _rBody.velocity = Vector3.zero;
        _rBody.angularVelocity = Vector3.zero;

        //18 Create method to find new position
        FindNewPosition();

        // -> IMPLEMENT END EPISODE WITH UPDATE
    }

    //04 Add OnActionReceived
    public override void OnActionReceived(float[] vectorAction)
    {
        //21 Add existential penalty
        SetReward(-0.005f);

        //22 Define the actions vectors
        //22.1 Move backward, forward or no action [3 Actions]
        float moveAction = vectorAction[0];

        //22.2 Rotate clockwise, counterclockwise, or no action [3 Actions]
        float rotateAction = vectorAction[1];

        //22.3 Rotate component around x clockwise, counterclockwise or no action [3 Actions]
        float xRotateAction = vectorAction[2];

        //22.4 Rotate component around y clockwise, counterclockwise or no action [3 Actions]
        float yRotateAction = vectorAction[3];

        //22.5 Rotate component around z clockwise, counterclockwise or no action [3 Actions]
        float zRotateAction = vectorAction[4];
        
        //23 Movement action
        if (moveAction == 1)
        {
            //24 Forward
            _rBody.AddForce(transform.forward * _moveFactor);
        }
        else if (moveAction == 2)
        {
            //25 Backward
            _rBody.AddForce(transform.forward * -_moveFactor);
        }

        //26 Rotation action
        if (rotateAction == 1)
        {
            //27 Clockwise
            _rBody.AddTorque(new Vector3(0, -_rotateFactor, 0));
        }
        else if (rotateAction == 2)
        {
            //28 Counterclockwise
            _rBody.AddTorque(new Vector3(0, _rotateFactor, 0));
        }

        //29 Check if agent is holding the component
        if (_holding != null)
        {
            //30 Reward agent for holding component
            SetReward(0.005f);

            //31 Component rotation in x
            if (xRotateAction == 1)
            {
                //32 Rotate component in x +90 degrees
                _holding.transform.localEulerAngles +=  new Vector3(90, 0, 0);
            }
            else if (xRotateAction == 2)
            {
                //33 Rotate component in x -90 degrees
                _holding.transform.localEulerAngles += new Vector3(-90, 0, 0);
            }

            if (yRotateAction == 1)
            {
                //34 Rotate component in y +90 degrees
                _holding.transform.localEulerAngles += new Vector3(0, 90, 0);
            }
            else if (yRotateAction == 2)
            {
                //35 Rotate component in y -90 degrees
                _holding.transform.localEulerAngles += new Vector3(0, -90, 0);
            }

            if (zRotateAction == 1)
            {
                //36 Rotate component in z +90 degrees
                _holding.transform.localEulerAngles += new Vector3(0, 0, 90);
            }
            else if (zRotateAction == 2)
            {
                //37 Rotate component in z -90 degrees
                _holding.transform.localEulerAngles += new Vector3(0, 0, -90);
            }
        }

        //38 Check if agent fell from platform
        if (transform.position.y < -1)
        {
            //39 Apply penalty
            SetReward(-0.5f);

            //40 Reset environment
            _environment.ResetEnvironment();

            //41 End episode
            EndEpisode();
        }
    }

    //05 Add Heuristic
    public override void Heuristic(float[] actionsOut)
    {
        //42 Set movement default as 0 (no action)
        actionsOut[0] = 0;

        //43 Use up arrow to move forward
        if (Input.GetKey(KeyCode.UpArrow))
        {
            actionsOut[0] = 1;
        }
        //44 Use down arrow to move backward
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            actionsOut[0] = 2;
        }

        //45 Set rotation default as 0 (no action)
        actionsOut[1] = 0;

        //46 Use Q to rotate counterclockwise
        if (Input.GetKey(KeyCode.Q))
        {
            actionsOut[1] = 1;
        }
        //47 Use E to rotate clockwise
        else if (Input.GetKey(KeyCode.E))
        {
            actionsOut[1] = 2;
        }

        //48 Set component rotation in X default as 0 (no action)
        actionsOut[2] = 0;

        //49 Use 1 to rotate +90
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            actionsOut[2] = 1;
        }
        //50 Use 2 to rotate -90
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            actionsOut[2] = 2;
        }

        //51 Set component rotation in Y default as 0 (no action)
        actionsOut[3] = 0;

        //52 Use 3 to rotate +90
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            actionsOut[3] = 1;
        }
        //53 Use 4 to rotate +90
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            actionsOut[3] = 2;
        }
        
        //54 Set component rotation in Z default as 0 (no action)
        actionsOut[4] = 0;

        //55 Use 5 to rotate +90
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            actionsOut[4] = 1;
        }
        //56 Use 6 to rotate -90
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            actionsOut[4] = 2;
        }
    }

    //06 Add CollectObservations
    public override void CollectObservations(VectorSensor sensor)
    {
        //79 Add the velocity of the rigidbody in X and Z[2 Observations]
        sensor.AddObservation(_rBody.velocity.x);
        sensor.AddObservation(_rBody.velocity.z);

        //80 Add the angular velocity of the rigidbody in Y[1 Observation]
        sensor.AddObservation(_rBody.angularVelocity.y);

        //81 Add the normalized position of the Agent[3 Observations]
        sensor.AddObservation(_environment.NormalizedPosition(transform.localPosition));

        //82 Add the normalized rotation of the Agent[3 Observations]
        sensor.AddObservation(transform.localRotation.eulerAngles / 360f);

        //83 Check if it is holding the component
        if (_holding != null)
        {
            //84 If yes, add the target's normalized position [3 Observations]
            sensor.AddObservation(_environment.NormalizedPosition(
                _environment.CurrentTarget.transform.localPosition));
        }
        else
        {
            //85 If not, add the component's normalized position[3 Observations]
            sensor.AddObservation(_environment.NormalizedPosition(
                _environment.CurrentComponent.transform.localPosition));
        }

        //86 Add the bool representing if the component is being held[1 Observation]
        sensor.AddObservation(_holding != null);
    }

    #region Other methods

    private void OnTriggerEnter(Collider other)
    {
        //57 Check if trigger is Component and if agent is not already holding component
        if (_holding == null &&
            other.gameObject.CompareTag("Component"))
        {
            //58 Reward agent
            SetReward(0.5f);
            
            //59 Define variable
            _holding = other.gameObject;

            //60 Deactivate colliders of component
            _holding.GetComponent<BoxCollider>().enabled = false;
            
            //61 Set agent as parent of component
            _holding.transform.parent = transform;

            //62 Move component in front of agent
            _holding.transform.position = (transform.position + transform.forward.normalized * 2f) + Vector3.up * 0.5f;
            
            //63 Reset the rotation of the component
            _holding.transform.localRotation = Quaternion.identity;
        }
    }

    //64 Implement OnTriggerStay to allow component placement
    private void OnTriggerStay(Collider other)
    {
        //65 Check if agent is holding component and trigger is Target
        if (_holding != null && other.gameObject.CompareTag("Target"))
        {
            //66 Reward agent if it is in trigger
            SetReward(0.1f);
            
            //67 Get the angle between the forward of target and component
            var forwardAngle = Vector3.Angle(_holding.transform.forward, _environment.CurrentTarget.transform.forward);

            //68 Get the angle between the up of the target and the component
            var updAngle = Vector3.Angle(_holding.transform.up, _environment.CurrentTarget.transform.up);

            //69 Check if both angles are less that error margin
            if (forwardAngle <= 30f && updAngle <= 30f)
            {
                //70 Reward agent success
                SetReward(1f);

                //71 Get target's transform
                var target = other.gameObject.transform.parent;
                
                //72 Get target's position
                var targetPosition = target.localPosition;

                //73 Get target's rotation
                var targetRotation = target.localRotation;

                //74 Destroy target
                Destroy(target.gameObject);

                //75 Set component parent as environment
                _holding.transform.parent = transform.parent;

                //76 Assign rotation and position of target to component
                _holding.transform.localPosition = targetPosition;
                _holding.transform.localRotation = targetRotation;

                //77 End episode
                EndEpisode();
            }
        }
    }

    /// <summary>
    /// Finds a new position of the agent
    /// </summary>
    private void FindNewPosition()
    {
        //19 Zero rotation
        transform.localRotation = Quaternion.identity;
        
        //20 Get a new position from the environment and assign it to agent
        Vector3 newPosition = _environment.FindValidPosition();
        transform.localPosition = newPosition;
    }

    #endregion
}
