using PhotoBooth.Cards;
using System;
using System.Collections.Generic;

namespace PhotoBooth
{
    public class PhotoCardTemplateInfo
    {
        public PhotoCardTemplateInfo()
        {
        }

        public Type TemplateType
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public PhotoCardTemplate CreateTemplate()
        {
            if (this.TemplateType == null)
            {
                throw new InvalidOperationException();
            }

            PhotoCardTemplate template = Activator.CreateInstance(this.TemplateType) as PhotoCardTemplate;
            template.DisplayName = this.DisplayName;
            return template;
        }

        public static List<PhotoCardTemplateInfo> GetTemplateListing()
        {
            List<PhotoCardTemplateInfo> templateList = new List<PhotoCardTemplateInfo>();
            templateList.Add(new PhotoCardTemplateInfo()
            {
                TemplateType = typeof(TwoByTwoCardTemplate),
                DisplayName = "2 x 2 Card"
            });

            templateList.Add(new PhotoCardTemplateInfo()
            {
                TemplateType = typeof(ThreeSmallOneLargeCardTemplate),
                DisplayName = "3 x 1 Card"
            });

            templateList.Add(new PhotoCardTemplateInfo()
            {
                TemplateType = typeof(MirrorCardTemplate),
                DisplayName = "Mirrored"
            });

            return templateList;
        }
    }
}
