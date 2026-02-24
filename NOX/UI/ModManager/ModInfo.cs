using NOX.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace NOX.UI.ModManager;

class ModInfo : MonoBehaviour
{
    class ModInfoTag : MonoBehaviour
    {
        Image bg;
        Outline bg_outline;
        Text label;


    }

    Text title;
    Text desc;
    Text author;
    Text downloads;

    Button download_button;
    Text dl_button_text;

    ContentSizeFitter fitter;

    VerticalLayoutGroup vertical;
    HorizontalLayoutGroup top;
    HorizontalLayoutGroup bottom;

    NOMMod mod;

    void Awake()
    {
        fitter = gameObject.AddComponent<ContentSizeFitter>(); //this.CreateChildObjectWithComponent<ContentSizeFitter>("NOXFitter");
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
        
        vertical = gameObject.AddComponent<VerticalLayoutGroup>();
        top = this.CreateChildObjectWithComponent<HorizontalLayoutGroup>("Header");
        desc = this.CreateChildObjectWithComponent<Text>("Description");
        desc.font = Resources.UIFont;
        bottom = this.CreateChildObjectWithComponent<HorizontalLayoutGroup>("Footer");

        title = top.CreateChildObjectWithComponent<Text>("Title");
        title.font = Resources.RegularFont;
        title.fontSize = 30;
        author = top.CreateChildObjectWithComponent<Text>("Author");
        author.font = Resources.RegularFont;

        var image = bottom.CreateChildObjectWithComponent<Image>("DownloadButton");
        image.color = Color.green;
        download_button = image.gameObject.AddComponent<Button>();
        var button_fit = image.gameObject.AddComponent<ContentSizeFitter>();
        button_fit.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        button_fit.verticalFit = ContentSizeFitter.FitMode.MinSize;

        dl_button_text = image.CreateChildObjectWithComponent<Text>("Label");
        dl_button_text.font = Resources.RegularFont;
        dl_button_text.text = "Download";

        downloads = bottom.CreateChildObjectWithComponent<Text>("Downloads");
        downloads.font = Resources.RegularFont;

    }

    public void SetMod(NOMMod mod)
    {
        title.text = mod.displayName;
        author.text = string.Join(", ", mod.authors);
        downloads.text = mod.downloadCount.ToString();
        desc.text = mod.description;
    }
}