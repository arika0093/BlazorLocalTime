# BlazorLocalTime

`BlazorLocalTime`は、Blazorで`DateTime`をユーザーのローカルタイムゾーンに変換するための機能を提供します。

## 使い方

### インストール
NuGetから`BlazorLocalTime`をインストールします。

```bash
dotnet add package BlazorLocalTime
```

次に、`Program.cs`に以下のコードを追加して、サービスを登録します。

```csharp
builder.Services.AddBlazorLocalTimeService();
```

最後に、`App.razor`(または`MainLayout.razor`など)に以下のコンポーネントを追加します。

```razor
<BlazorLocalTimeProvider />
```

詳細は、以下のサンプルを参照してください。
* [example]()
* [example/Program.cs]()
* [example/App.razor]()

### Componentとして使う

単純にローカル時刻で文字として表示したい場合は`LocalTimeText`コンポーネントを使用します。

```razor
<LocalTimeText Value="@DateTime.UtcNow" Format="yyyy/MM/dd HH:mm:ss" />
```

または、`LocalTime`を使用して以下のようにコンポーネント直下で受け取ることもできます。

```razor
<LocalTime Value="@DateTime.UtcNow" Context="dt">
    @dt.ToString("yyyy/MM/dd HH:mm:ss")
</LocalTime>
```

入力フォーム等において、ローカル時刻で表示、UTC時刻で保存したい場合は以下のように使います。

```razor
<LocalTime @bind-Value="dt" Context="dtl">
    <DateTimeInput @bind-Value="dtl" />
</LocalTime>

@code {
    private DateTime dt = DateTime.UtcNow;
}
```

### Serviceとして使う
`BlazorLocalTimeService`を使用して、コード側でローカルタイムゾーンに変換することもできます。

```razor
@inject ILocalTimeService LocalTimeService
@code {
    protected override void OnInitialized()
    {
        var localNow = LocalTimeService.ToLocalTime(DateTime.UtcNow);
    }
}
```


## 参考文献

[この記事](https://www.meziantou.net/convert-datetime-to-user-s-time-zone-with-server-side-blazor-time-provider.htm)を大いに参考にさせていただきました。  
実際のところ、このライブラリは上記の内容を毎回書きたくないために作成されています。