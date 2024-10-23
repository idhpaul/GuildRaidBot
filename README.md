# 🤖 Guild Raid Bot(GRB)

--------
## 📍 Index
- ⚙️ [Required settings](https://github.com/idhpaul/GuildRaidBot?tab=readme-ov-file#%EF%B8%8F-required-settings)
- 📚 [Packages](https://github.com/idhpaul/GuildRaidBot?tab=readme-ov-file#-packages)
- 📸 [ScreenShot](https://github.com/idhpaul/GuildRaidBot?tab=readme-ov-file#-screenshot)
------
## ⚙️ Required settings

### 1) Role
* __Define ``Admin role``__
    > ⚠️ __You must setting same role name.__
    > * discord channel setting([How to make Role](https://support.discord.com/hc/en-us/articles/206029707-Setting-Up-Permissions-FAQ))
    > * src([Enum/Role.cs](https://github.com/idhpaul/GuildRaidBot/blob/master/Enum/Role.cs)) 
### 2) Channel & Category
* __Create ``3 Channel`` and ``1 Category``__
    > __Each ID must be declared in secrets.json.__
### 3) secrets.json ([ref : App-Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#user-secrets-in-non-web-applications))
```
{
    "DiscordToken": "<input : discord channel bot token>",
    "Config": {
        "GuildID": 00000000000000,
        "RegisterChannelID": 00000000000000,
        "ConfirmChannelID": 00000000000000,
        "InquireCategoryID": 00000000000000,
        "SqliteDbName": "<input : sqlite db name (ex:grb.sqlite)>"
    },
    "Dev": {
        "DiscordToken": "<input : develop discord channel bot token>",
        "Config": {
            "GuildID": 00000000000000,
            "RegisterChannelID": 00000000000000,
            "ConfirmChannelID": 00000000000000,
            "InquireCategoryID": 00000000000000,
            "SqliteDbName": "<input : sqlite db name (ex:grb_dev.sqlite)>"
        },
    }
}
```

## 📚 Packages
### Packages
- **Discord.Net**
- **Serilog**
- **Microsoft.Extensions.Configuration.UserSecrets** ([ref : App-Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#user-secrets-in-non-web-applications))
- **Microsoft.Extensions.Hosting** ([ref : Generic-Host](https://learn.microsoft.com/ko-kr/dotnet/core/extensions/generic-host?tabs=appbuilder))


## 📸 ScreenShot
- __레이드 신청__
![레이드 신청](https://i.imgur.com/96GCKRx.png)
- __레이드 신청 확인__
![신청 확인](https://i.imgur.com/sj0CgpJ.png)
- __신청 수락 확인(개인 채널)__
![신청 수락 확인 개인 채널](https://i.imgur.com/ZJQO00u.png)
- __신청 취소 쓰레드__
![신청 취소 쓰레드](https://i.imgur.com/WFHWGJM.png)
