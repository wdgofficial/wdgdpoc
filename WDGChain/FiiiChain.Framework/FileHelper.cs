// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace FiiiChain.Framework
{
    public static class FileHelper
    {
        public static void StringSaveFile(string str, string filePath)
        {
            using (FileStream fs = File.Create(filePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, str);
            }
        }

        public static string LoadFileString(string filePath)
        {
            string fileString = String.Empty;
            using (FileStream fs = File.OpenRead(filePath))
            {
                if (fs.CanRead)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    fileString = bf.Deserialize(fs).ToString();
                }
            }
            return fileString;
        }
    }
}
