using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Gene : MonoBehaviour
{
    abstract public string Name { get; }
    abstract public void Randomize();
    abstract public void Inherit(Gene gene);
}

