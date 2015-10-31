using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;
using System.Text;
using UnityEngine.UI;

public class Identicon : MonoBehaviour {

	[SerializeField]GameObject rect_plane;//描画につかうPlane
	[SerializeField]string code;

	string precode;

	// Use this for initialization
	void Start () {
		MakeIdenticon(code);
		precode = code;
	}
	
	// Update is called once per frame
	void Update () {
		//文字更新すると変化するようにする
		if(precode != code){
			//Destroy child
			foreach ( Transform n in gameObject.transform ){
				GameObject.Destroy(n.gameObject);
			}
			MakeIdenticon(code);
			precode = code;
		}
	}
	
	void MakeIdenticon(string code){
		byte[] data = Encoding.UTF8.GetBytes(code);

		//SHA1でハッシュ値取得
		SHA1Managed sha1 = new SHA1Managed();
		byte[] hash = sha1.ComputeHash(data);
		//リソース解放
		sha1.Clear();

		StringBuilder strbuilder = new StringBuilder();
		foreach(byte b in hash){
			strbuilder.Append(b.ToString("x2"));
		}
		string hash_str = strbuilder.ToString();

		bool[,] pattern = CreatePattern(hash_str);

		MakeIdenticonPlane(pattern);
	}

	//2次元配列を返す
	bool[,] CreatePattern(string hash){
		bool[,] pattern = new bool[5,5];

		int counter = 0;
		char digit;

		//パターンの半分を生成
		for(int i=0;i<3;i++){
			for(int j=0;j<5;j++){
				digit = hash[counter];

				//桁の数字が偶数ならtrue,奇数ならfalse
				//偶数の時
				if(Convert.ToInt32(digit.ToString(),16) % 2 == 0){
					pattern[i,j] = true;
				}else{//奇数の時
					pattern[i,j] = false;
				}
				counter++;

			}
		}

		//もう半分にコピー
		for(int i=3;i<5;i++){
			for(int j=0;j<5;j++){
				pattern[i,j] = pattern[4-i,j];
			}
		}

		return pattern;
	}


	void MakeIdenticonPlane(bool[,] pattern){

		for(int i=0;i<5;i++){
			for(int j=0;j<5;j++){
				GameObject gobj = Instantiate(rect_plane);
				gobj.name = i.ToString() + "-" + j.ToString();
				gobj.transform.parent = gameObject.transform;

				//planeは10単位の大きさなので
				gobj.transform.position = new Vector3(gameObject.transform.lossyScale.x * i*10,0,gameObject.transform.lossyScale.z * j*10);

				if(pattern[i,j]){
					gobj.GetComponent<Renderer>().material.color = Color.black;
				}else{
					gobj.GetComponent<Renderer>().material.color = Color.white;
				}

			}
		}
	}
}
