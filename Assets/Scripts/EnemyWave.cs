using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pheryus { 
    [CreateAssetMenu(fileName = "Wave", menuName = "New Enemy Wave")]
    public class EnemyWave : ScriptableObject {

        public Option[] option;
        public int difficult;

        public Option GetRandomOption() {
            return option[Random.Range(0, option.Length - 1)];
        }
    }
}