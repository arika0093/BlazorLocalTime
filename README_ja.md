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

最後に、`Routes.razor`(または`MainLayout.razor`など)に以下のコンポーネントを追加します。

```razor
@using BlazorLocalTime
<BlazorLocalTimeProvider />
```

詳細は、以下のサンプルを参照してください。
* [BlazorLocalTimeExample](./example/BlazorLocalTimeExample)
  * [Program.cs](./example/BlazorLocalTimeExample/Program.cs)
  * [Routes.razor](./example/BlazorLocalTimeExample/Components/Routes.razor)
  * [Home.razor](./example/BlazorLocalTimeExample/Components/Pages/Home.razor)

### Componentとして使う

単純にローカル時刻で文字として表示したい場合は`LocalTimeText`コンポーネントを使用します。

```razor
<LocalTimeText Value="@DateTime.UtcNow" Format="yyyy/MM/dd HH:mm:ss" />
```

または、`LocalTime`を使用してコンポーネント直下で変換後の値を受け取ることもできます。

```razor
<LocalTime Value="@DateTime.UtcNow" Context="dt">
    @dt.ToString("yyyy/MM/dd HH:mm:ss")
</LocalTime>
```

入力フォームでは、表示する値はローカル時刻、保存する値はUTCとすることが一般的です。  
`LocalTimeForm`コンポーネントを使用することでそのようなフォームを簡単に作成できます。

```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.DateTimeLocal"
        Value="dtf.Value" ValueChanged="dtf.ValueChanged"
        ValueExpression="() => dtf.Value" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```

入力フォームは日付・時刻の分割入力にも対応しています。

```razor
<LocalTimeForm @bind-Value="Dt" Context="dtf">
    <InputDate Type="InputDateType.Date" Value="dtf.Date" ValueExpression="() => dtf.Date" ValueChanged="dtf.DateChanged" />
    <InputDate Type="InputDateType.Time" Value="dtf.Time" ValueExpression="() => dtf.Time" ValueChanged="dtf.TimeChanged" />
</LocalTimeForm>

@code {
    private DateTime Dt { get; set; } = DateTime.UtcNow;
}
```

### Serviceとして使う
`ILocalTimeService`を使用して、コード側で変換することもできます。

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
