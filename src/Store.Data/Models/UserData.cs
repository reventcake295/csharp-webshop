using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Store.Common.Enums;
using Store.Common.Interfaces;
using Store.Common.Model;
using Store.Data.Attributes;
using Store.Data.Enums;

namespace Store.Data.Models;

[Table("users")]
internal class UserData : IEntityModel<UserModel, UserData>
{
    internal UserData(UserModel model)
    {
        user_id = model.Id;
        username = model.Username;
//        password = model.Password;
        email = model.Email;
        adres_street = model.AdresStreet;
        adres_number = model.AdresNumber;
        adres_add = model.AdresAdd;
        adres_postal = model.AdresZip;
        adres_city = model.AdresCity;
        auth_id = (int)model.Perm;
    }

    internal UserData(int user_id, string username, string password, string email, string adres_street, int adres_number, string adres_add, string adres_postal, string adres_city, int auth_id)
    {
        this.user_id = user_id;
        this.username = username;
        this.password = password;
        this.email = email;
        this.adres_street = adres_street;
        this.adres_number = adres_number;
        this.adres_add = adres_add;
        this.adres_postal = adres_postal;
        this.adres_city = adres_city;
        this.auth_id = auth_id;
    }
    
    internal UserData(UserModel model, string password) : this(model)
    {
        this.password = password;
    }
    
    [ModelMapping(Name = "Id")]
    [Key, Required] 
    public int user_id { get; init; }
    public int GetId() => user_id;
    
    [ModelMapping(Name = "Username")]
    [Required, Index(IsUnique = true), MaxLength(255)]
    public string username { get; set; }
    
    [ModelMapping(Name = "Disabled", PropertyType = PropertyType.Disabled)]
    [Required, PasswordPropertyText, MaxLength(255)]
    public string? password { get; set; }
    
    [ModelMapping(Name = "Email")]
    [Required, EmailAddress, MaxLength(255)]
    public string email { get; set; }
    
    [ModelMapping(Name = "AdresStreet")]
    [Required, MaxLength(255)]
    public string adres_street { get; set; }
    
    [ModelMapping(Name = "AdresNumber")]
    [Required]
    public int adres_number { get; set; }
    
    [ModelMapping(Name = "AdresAdd")]
    [Required, MaxLength(255)]
    public string adres_add { get; set; }
    
    [ModelMapping(Name = "AdresZip")]
    [Required]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Postal code must be 6 characters.")]
    public string adres_postal { get; set; }
    
    [ModelMapping(Name = "AdresCity")]
    [Required, MaxLength(255)]
    public string adres_city { get; set; }
    
    [ModelMapping(Name = "Perm", PropertyType = PropertyType.Enum)]
    [Required]
    public int auth_id { get; set; }

    public UserModel ToCommonModel() => new(
        user_id,
        username,
//        password,
        email,
        adres_street,
        adres_number,
        adres_add,
        adres_city,
        adres_postal,
        (Perm) auth_id
    );
}