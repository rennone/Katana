using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Platforms
{
    public class Patroller : MonoBehaviour
    {
        // 巡回する頂点群
        [SerializeField]
        protected Transform[] _vertices;

        // 上記の配列を順方向に回るか逆方向に回るか
        [SerializeField] 
        protected Boolean _forwardDirection = true;

        [SerializeField]
        protected float _speed = 10.0f;

        // 上記で設定されたTransformがこのオブジェクトの子だったりすると一緒に移動してしまうので最初に初期位置を保存しておく
        
        List<Vector3> _verticesWorldPosition = new List<Vector3>();

        protected int _nextIndex = 0;


        // 目的地
        protected Vector3 Destination { get { return _verticesWorldPosition[_nextIndex]; } }

        // 次の頂点を目指す
        protected void Next()
        {
            _nextIndex = (_forwardDirection ? _nextIndex + 1 : _nextIndex + _vertices.Count() - 1) % _vertices.Count();
        }

        // Use this for initialization
        protected virtual void Start ()
        {
            foreach (var tr in _vertices)
            {
                _verticesWorldPosition.Add(tr.position);
            }
            transform.position = Destination;
        }
	
        // Update is called once per frame
        protected virtual void Update ()
        {
            var distance = Destination - transform.position;

            // 移動量
            var offset = _speed*Time.deltaTime; //TODO : Timeを書き換え

            // 次に進む
            if (distance.sqrMagnitude <= offset*offset)
            {
                transform.position = Destination;

                Next();
                return;
            }

            // 進む
            transform.position += offset*distance.normalized;
        }
    }
}
