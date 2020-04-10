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

