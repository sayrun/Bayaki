﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bayaki.Properties {
    using System;
    
    
    /// <summary>
    ///   ローカライズされた文字列などを検索するための、厳密に型指定されたリソース クラスです。
    /// </summary>
    // このクラスは StronglyTypedResourceBuilder クラスが ResGen
    // または Visual Studio のようなツールを使用して自動生成されました。
    // メンバーを追加または削除するには、.ResX ファイルを編集して、/str オプションと共に
    // ResGen を実行し直すか、または VS プロジェクトをビルドし直します。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   このクラスで使用されているキャッシュされた ResourceManager インスタンスを返します。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Bayaki.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   厳密に型指定されたこのリソース クラスを使用して、すべての検索リソースに対し、
        ///   現在のスレッドの CurrentUICulture プロパティをオーバーライドします。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   &lt;html&gt;
        ///&lt;script type=&quot;text/javascript&quot; src=&quot;http://maps.google.com/maps/api/js?sensor=false&quot;&gt;&lt;/script&gt;
        ///&lt;script type=&quot;text/javascript&quot;&gt;
        ///var map;
        ///var marker = null;
        ///
        ///function initialize() {
        ///	// 東京都庁の座標
        ///	var y = 35.689634;
        ///	var x = 139.692101;
        ///	var latlng = new google.maps.LatLng(y, x);
        ///	var opts = {
        ///			zoom: 14,
        ///			center: latlng,
        ///			mapTypeId: google.maps.MapTypeId.ROADMAP
        ///		};
        ///	map = new google.maps.Map(document.getElementById(&quot;gmap&quot;), opts);
        ///}
        ///
        ///function movePos( x, y) {
        ///	var pos = new go [残りの文字列は切り詰められました]&quot;; に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string googlemapsHTML {
            get {
                return ResourceManager.GetString("googlemapsHTML", resourceCulture);
            }
        }
        
        /// <summary>
        ///   型 System.Drawing.Bitmap のローカライズされたリソースを検索します。
        /// </summary>
        internal static System.Drawing.Bitmap GPXLoader_ICON {
            get {
                object obj = ResourceManager.GetObject("GPXLoader_ICON", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   GPXLoader に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string GPXLoader_NAME {
            get {
                return ResourceManager.GetString("GPXLoader_NAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   削除してもよいですか？ に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string MSG1 {
            get {
                return ResourceManager.GetString("MSG1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Skytraq Downloader に類似しているローカライズされた文字列を検索します。
        /// </summary>
        internal static string SkytraqDownloader_NAME {
            get {
                return ResourceManager.GetString("SkytraqDownloader_NAME", resourceCulture);
            }
        }
    }
}
