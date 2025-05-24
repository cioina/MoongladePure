﻿using System.ComponentModel.DataAnnotations;

namespace MoongladePure.Configuration;

public class GeneralSettings : IBlogSettings
{
    [Required]
    [Display(Name = "Meta keyword")]
    [MaxLength(1024)]
    public string MetaKeyword { get; set; }

    [Required]
    [Display(Name = "Logo text")]
    [MaxLength(16)]
    public string LogoText { get; set; }

    [Required]
    [RegularExpression(@"[a-zA-Z0-9\s.\-\[\]]+", ErrorMessage = "Only letters, numbers, - and [] are allowed.")]
    [Display(Name = "Copyright")]
    [MaxLength(64)]
    public string Copyright { get; set; }

    [Required]
    [Display(Name = "Blog title")]
    [MaxLength(16)]
    public string SiteTitle { get; set; }

    [Required]
    [Display(Name = "Your name")]
    [MaxLength(32)]
    public string OwnerName { get; set; }

    [Required]
    [Display(Name = "Owner email")]
    [DataType(DataType.EmailAddress)]
    [MaxLength(64)]
    public string OwnerEmail { get; set; }

    [Required]
    [Display(Name = "Your description")]
    [DataType(DataType.MultilineText)]
    [MaxLength(256)]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Short description")]
    [MaxLength(32)]
    public string ShortDescription { get; set; }

    [Display(Name = "Side bar HTML code")]
    [DataType(DataType.MultilineText)]
    [MaxLength(2048)]
    public string SideBarCustomizedHtmlPitch { get; set; }

    [Display(Name = "Side bar display")]
    public SideBarOption SideBarOption { get; set; }

    [Display(Name = "Footer HTML code")]
    [DataType(DataType.MultilineText)]
    [MaxLength(4096)]
    public string FooterCustomizedHtmlPitch { get; set; }

    [Display(Name = "Auto Light / Dark theme regarding client system settings")]
    public bool AutoDarkLightTheme { get; set; }

    public int ThemeId { get; set; } = 1;

    [Display(Name = "Profile")]
    public bool WidgetsProfile { get; set; } = true;

    [Display(Name = "Tags")] 
    public bool WidgetsTags { get; set; } = true;

    [Display(Name = "Categories")] 
    public bool WidgetsCategoryList { get; set; } = true;

    [Display(Name = "Friend links")]
    public bool WidgetsFriendLink { get; set; } = true;

    [Display(Name = "Subscription buttons")]
    public bool WidgetsSubscriptionButtons { get; set; } = true;

    [MaxLength(64)]
    public string AvatarUrl { get; set; }
}

public enum SideBarOption
{
    Right = 0,
    Left = 1,
    Disabled = 2
}