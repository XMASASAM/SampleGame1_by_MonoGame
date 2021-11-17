using System.IO.IsolatedStorage;
using System.IO;
using Newtonsoft.Json;
namespace MDclass.StorageClass
{ 
    /// <summary>
    /// インスタンスを保存するクラス
    /// </summary>
    static public class StorageClass
    {
        /// <summary>
        /// データを保存
        /// </summary>
        /// <typeparam name="T">保存したいインスタンスの型</typeparam>
        /// <param name="Key">データを呼び出す合言葉</param>
        /// <param name="data">保存したいインスタンス</param>
        static public void Save<T>(string Key,T data)
        {
            //ファイルへ書き込む準備
            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(Key, FileMode.OpenOrCreate, FileAccess.Write);
            //ファイルへ書き込む
            using (StreamWriter sw = new StreamWriter(isoStream))
            {
                sw.WriteLine(JsonConvert.SerializeObject(data));
            }
        }

        /// <summary>
        /// 保存したデータを出力
        /// </summary>
        /// <typeparam name="T">保存したインスタンスの型</typeparam>
        /// <param name="Key">データを呼び出す合言葉</param>
        static public T Load<T>(string Key)
        {
            T data;
            //ファイルを読み込む準備
            IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(Key, FileMode.OpenOrCreate, FileAccess.Read);
            //ファイルを読み込む
            using (StreamReader sr = new StreamReader(isoStream))
            {
                data = JsonConvert.DeserializeObject<T>(sr.ReadLine());
            }
            //取得したデータを返す
            return data;

        }

        /// <summary>
        /// 合言葉が存在しているか
        /// </summary>
        /// <param name="Key">データを呼び出す合言葉</param>
        /// <returns></returns>
        static public bool Exist(string Key)
        {
            //準備
            IsolatedStorageFile savegameStorage = IsolatedStorageFile.GetUserStoreForDomain();
            //結果
            return savegameStorage.FileExists(Key);
        }


    }


    
}
