using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;
using Managers;
using System;

namespace Characters
{
    public class CharacterRange : MonoBehaviour
    {
        private BaseCharacter myCharacter;
        public float totalRange;
        public float distancePerRange = 0.3f;
        public List<BaseCharacter> enemiesInRange;
        public void Start()
        {
            if (GetComponent<BaseCharacter>() != null)
            {
                myCharacter = GetComponent<BaseCharacter>();
                totalRange = myCharacter.unitInformation.range * distancePerRange;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (myCharacter.isFighting)
            {
                CheckAttackRange();

            }
        }

        public void OnDrawGizmos()
        {
            if(myCharacter != null && myCharacter.isFighting)
            {
                DrawRange();
            }
        }

        public void DrawRange()
        {
            Gizmos.color = Color.red;
            Vector2 direction = myCharacter.myMovements.CheckDirection();


            Vector2 endLine = new Vector2(transform.position.x + (direction.x * totalRange), transform.position.y);

            Gizmos.DrawLine(transform.position, endLine);
        }
        public void CheckAttackRange()
        {
            Vector2 direction = myCharacter.myMovements.CheckDirection();
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, direction, 0.3f);

            if(enemiesInRange == null)
            {
                enemiesInRange = new List<BaseCharacter>();
            }

            for (int i = 0; i < hit.Length; i++)
            {
                BaseCharacter unitHit = hit[i].collider.gameObject.GetComponent<BaseCharacter>();
                if (unitHit != null)
                {
                    if (unitHit == myCharacter)
                        continue;

                    if(unitHit.teamType != TeamType.Neutral && unitHit.teamType != myCharacter.teamType)
                    {
                        if(!enemiesInRange.Contains(unitHit))
                        {
                            enemiesInRange.Add(unitHit);
                        }
                    }
                }
            }

            if(enemiesInRange.Count > 0)
            {
                // Remove Death and Injured Enemies
                enemiesInRange.RemoveAll(x => x.unitInformation.currentState != UnitState.Healthy);

                if (enemiesInRange.Count > 0)
                {
                    myCharacter.EnemyInRange();
                }
            }
        }
    }

}
