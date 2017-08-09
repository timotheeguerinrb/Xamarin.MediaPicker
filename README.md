# Multiple Media Picker :boom: :star2:
### This Xamarin library is created from an existing Android library https://github.com/erikagtierrez/multiple-media-picker created by @erikagtierrez
### An android library to pick multiple images from built-in gallery. This library is encouraged to use as little memory as possible. 

![](https://img.shields.io/badge/license-APACHE%202-ff69b4.svg)

![](https://raw.githubusercontent.com/erikagtierrez/multiple-media-picker/master/cover.jpg)

# Usage
Include easily in your project adding the dependency to your packages.config file.  

```config
<package id="Xamarin.MediaPicker" version="1.0.0.2" targetFramework="monoandroid71" />
```
# Getting started
In the activity from where you want to call the library, declare

```C#
  static int OPEN_MEDIA_PICKER = 1;  // Request code
```

and request permissions to read external storage

```config
  Manifest.permission.READ_EXTERNAL_STORAGE
```

Create the intent

```C#
  Intent intent = new Intent(Context, typeof(MediaGallery));
  intent.PutExtra("title", "Select your photo");
  intent.PutExtra("maxNumberSelection", 1);
  StartActivityForResult(intent, OPEN_MEDIA_PICKER);
```

and override onActivityResult 

```C#
public override void OnActivityResult(int requestCode, int resultCode, Intent data)
{
    base.OnActivityResult(requestCode, resultCode, data);
    if (requestCode == OPEN_MEDIA_PICKER)
    {
        if ((Result)resultCode == Result.Ok)
        {
            var selectionResult = data.GetStringArrayListExtra("result");
            // selectionResult is the url of selected image
        }
    }
}
```

## Custom styles

The colors will be inherited from the class it was called.

# License

```
Copyright 2016 Erika Gutierrez

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```
