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
        [SerializeField] private Transform[] _vertices;

        // 上記の配列を順方向に回るか逆方向に回るか
        [SerializeField] private Boolean _forwardDirection = true;

        [SerializeField] private float _speed = 10.0f;

        // 上記で設定されたTransformがこのオブジェクトの子だったりすると一緒に移動してしまうので最初に初期位置を保存しておく
        
        List<Vector3> _verticesWorldPosition = new List<Vector3>(); 

        private int _destinationIndex = 0;


        Vector3 Destination { get { return _verticesWorldPosition[_destinationIndex]; } }

        void Next()
        {
            _destinationIndex = (_forwardDirection ? _destinationIndex + 1 : _destinationIndex + _vertices.Count() - 1) % _vertices.Count();
        }
        // Use this for initialization
        void Start ()
        {
            foreach (var tr in _vertices)
            {
                _verticesWorldPosition.Add(tr.position);
            }
            transform.position = Destination;
        }
	
        // Update is called once per frame
        void Update ()
        {
            var distance = Destination - transform.position;

            // 移動量
            var offset = _speed*Time.deltaTime; //TODO : Timeを書き換え

          //  Debug.Log(_vertices[0].position.ToString() + " - " + transform.position.ToString() + " - " + _vertices[1].position.ToString());
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
