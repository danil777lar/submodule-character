using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

namespace Larje.Character
{
	public class Character : MonoBehaviour
	{
		[field: SerializeField] public Type CharacterType { get; private set; }
	}

	public enum Type
	{
		Player,
		NPC
	}
}