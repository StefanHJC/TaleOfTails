using Core.Database;
using UnityEngine;

namespace Utils
{
    public class UnitSpawnTag : MonoBehaviour
    {
        [SerializeField] private UnitTypeId _typeId;
        [SerializeField] private int _teamId;

        public UnitTypeId UnitType
        {
            get => _typeId;
            set => _typeId = value;
        }

        public int TeamId
        {
            get => _teamId;
            set => _teamId = value;
        }
    }
}