# Contributing

If you want to contribute, please participate in discussions on GitHub issues or create a fork and submit me a pull request. 

# Translations

When working on the UI, keep in mind that the application is translated. The main (neutral) language is french and there are english and italian translations. So it is not possible to use hard-coded text, you have to use the TextResource markup extension like this :

```xaml
<TextBlock Text="{utils:TextResource resource_name}" />
```

*resource_name* correspond to the name of the resource to load from resources.resx. 

So if you need a new text resource for the UI, please add a line in resources.resx and give it an explicit name (in english, sorry for existing ones that are in french). You can leave the text value in english, I will make the translation.

> If you want to make translations yourself, you will have to install [Multilingual App Toolkit for VS2017+ extension](https://marketplace.visualstudio.com/items?itemName=MultilingualAppToolkit.MultilingualAppToolkit-18308).

> Please DO NOT modify resources.en.resx and resources.it.resx directly. These are generated files. Only modify resources.resx. You can add your text in english and I will make the translations.

# Troubleshouting

## Error at first build

There is sometimes an error `A numeric comparison was attempted on "$(MsBuildMajorVersion)" that evaluates to "" instead of a number, in condition "($(MsBuildMajorVersion) < 16)"` at first build, I don't know why. You can ignore it and launch the application, the error will just disappear.

## XamlParseException at startup

If you modify a ViewModel, sometimes it can cause a XamlParseException at startup:

```
System.Windows.Markup.XamlParseException
  HResult=0x80131501
  Message=Impossible de charger le fichier ou l'assembly 'PropertyChanged, Version=3.2.8.0, Culture=neutral, PublicKeyToken=ee3ee20bcf148ddd' ou une de ses dépendances. Le fichier spécifié est introuvable.
  Source=PresentationFramework
  StackTrace:
   at System.Windows.Markup.WpfXamlLoader.Load(XamlReader xamlReader, IXamlObjectWriterFactory writerFactory, Boolean skipJournaledProperties, Object rootObject, XamlObjectWriterSettings settings, Uri baseUri)
```

The exception seems originate from Fody.PropertyChanged not found despite the package is correctly installed and restored. To solve the problem you have to rebuild the solution. Just click on "Build > Rebuild Solution" menu item and the error will go away.



