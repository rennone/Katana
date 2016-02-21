using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Katana
{
    public class ActorHolder : MonoBehaviour
    {

        // このラインを越えたら強制的に破壊される高さ
        [SerializeField] private int borderHeightToDeath = -200;


        public Player Player { get; private set; }

        // アクターのインスタンスリスト
        // key   : tagName
        // value : Actorリスト 
        private readonly Dictionary<string, HashSet<Actor>> _actorList = new Dictionary<string, HashSet<Actor>>();

        private void SetPlayer(Player player)
        {
            if (Player != null)
                Destroy(Player);
            Player = player;
        }


        public void RegisterActor(Actor actor)
        {
            if (!_actorList.ContainsKey(actor.tag))
                _actorList[actor.tag] = new HashSet<Actor>();

            _actorList[actor.tag].Add(actor);

            // Playerだけは特別に保存しておく(アクセスが多いので)
            if (actor.tag == TagName.Player)
                SetPlayer(actor.gameObject.GetComponent<Player>());
        }

        public void RemoveActor(Actor actor)
        {
            if (!_actorList.ContainsKey(actor.tag))
                return;

            _actorList[actor.tag].Remove(actor);
        }

        // Update is called once per frame
        private void Update()
        {
            foreach (var actorSet in _actorList)
            {
                foreach (var actor in actorSet.Value)
                {
                    if (actor.transform.position.y <= borderHeightToDeath)
                        Destroy(actor.gameObject);
                }
            }
        }

        void OnDestroy()
        {
            foreach (var actorSet in _actorList)
            {
                while (actorSet.Value.Count != 0)
                {
                    var actor = actorSet.Value.First();
                    DestroyImmediate(actor.gameObject);
                }
            }
        }
    }
}