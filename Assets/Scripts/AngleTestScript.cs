using System.Collections.Generic;
using UnityEngine;

public class AngleTestScript : MonoBehaviour
{
    //[SerializeField][Tooltip("Ugly button")] bool _Actualize = false;
    [SerializeField] Transform _target;

    [Header("Targets :")]
    [SerializeField] Transform[] _bones;

    [Header("Objects :")]
    [SerializeField] Transform[] _obj;

    [Header("Distances :")]
    [SerializeField] List<float> _distances;

    private void Start()
    {
        GetDistance();
    }

    private void FixedUpdate()
    {
        //if (_Actualize)
        //{
        ResetValues();
        Process();
        //    _Actualize = false;
        //}
    }

    void Process()
    {
        // Vecteur AC && normalisation de AC
        Vector3 AC = _obj[2].position - _obj[0].position;
        float distanceAC = AC.magnitude;
        distanceAC = Mathf.Clamp(distanceAC, 0.0001f, _distances[0] + _distances[1] - 0.0001f); // Clamp la distance pour �viter des erreurs si la cible est trop loin

        Vector3 AC_normalized = AC.normalized;

        // Loi des cosinus pour trouver l'angle au coude
        float cosTheta = CosinusLaw(_distances[0], _distances[1], distanceAC);
        float theta = Mathf.Acos(cosTheta); // Angle en radians

        // Trouver un vecteur perpendiculaire � AC
        Vector3 axis = Vector3.Cross(Vector3.up, AC_normalized).normalized; // Utilise "up" par d�faut

        if (axis.magnitude < 0.001f) // Si "up" est parall�le � AC, prend un autre axe
            axis = Vector3.Cross(Vector3.right, AC_normalized).normalized;

        // Rotation de AC pour positionner B
        Quaternion rotation = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, axis);
        Vector3 AB = rotation * AC_normalized * _distances[0];

        _target.position = _obj[0].position + AB;

        LookBones();
    }

    void GetDistance()
    {
        for (int i = 0; i < _bones.Length - 1; i++)
        {
            _distances.Add((_bones[i].position - _bones[i + 1].position).magnitude);
        }
    }

    void LookBones()
    {
        if (_bones.Length != _obj.Length)
        {
            Debug.LogError("Not same number of bones and objects");
            return;
        }

        for (int i = 0; i < _bones.Length - 1; i++)
        {
            _bones[i].LookAt(_obj[i + 1].position);
        }
    }

    float CosinusLaw(float distance1, float distance2, float maxDistance)
    {
        return (distance1 * distance1 + maxDistance * maxDistance - distance2 * distance2) / (2 * distance1 * maxDistance);
    }

    void ResetValues()
    {
        for (int i = 0; i < _bones.Length; i++)
        {
            _bones[i].rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
