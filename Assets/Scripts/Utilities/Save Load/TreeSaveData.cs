using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARChristmas
{
    [Serializable]
    public class TreeSaveData
    {
        // save "Local(relative to the parent tree)" position, not "World" position
        public List<Vector3> decorationItemLocalPos;
        public List<Color> decorationItemColors;

        /// <summary>
        /// set saving data when creating save object
        /// </summary>
        public TreeSaveData(ChristmasTree christmasTree)
        {
            this.decorationItemLocalPos = christmasTree.decorationItemLocalPos;
            this.decorationItemColors = christmasTree.decorationItemColors;
        }
    }   
}