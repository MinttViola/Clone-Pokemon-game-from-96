using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Dialog
{
        [SerializeField] List<string> lines;

        public List<string> Lines { get {return lines; }  }
    
}
