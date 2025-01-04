---
layout: single
title: Emacs For The Win
date:   2022-04-18 19:28:43 +0200
categories: tech
subcategory: ide
description: Why I believe Emacs adds value
slug: emacs-for-the-win
---

![Emacs](/images/doom.png)

### WSL2 woes

My IDE of choice for the past year has been Emacs, more specfically [Doom Emacs](https://github.com/hlissner/doom-emacs). However in changing jobs I've found myself cornered on a Windows desktop. This has posed some challenges, specifically around having a fast and smooth development experience using Emacs. My get up and running approach was to install Emacs vanilla with a pre-built [binary](http://ftp.wayne.edu/gnu/emacs/windows/emacs-28/). 

But alas, this resulted in a 20+ second load time. So I chose the next best thing, WSL2. WSL2 on Windows 11 is an amazing option given that you can now leverage GTK. For instructions on doing this look no further then [Bozhidar Batsov](https://emacsredux.com/blog/2021/12/19/using-emacs-on-windows-11-with-wsl2/).

I followed this process, compiled Emacs 29 with PGTK and even added the hack for copying from Windows to Emacs. But I was disappointed with an experience of instability, where Emacs would occassionally crash (without warning) and at times copying being sporadic or not working in certain screens. But the real killer was the networking from WSL2 to my Windows Host. 

While everything worked largely, I run a VPN for work purposes. I solved that largely using [WSL VpnKit](https://github.com/sakai135/wsl-vpnkit). This however broke at times. Thereafter further complications resulted from requiring OpenSSL certificates found in Windows that were not in WSL2. So between random crashes, sporadic network failures and configuration pains was a huge loss of time. And for that I ditched Emacs, and moved to VSCode. 

### Emacs joy

But despite the smooth experience and easy configuration of VsCode, I just couldn't push past the missing workflow, the keyboard shortcuts and the design. I will take my hat of to the amazing VsCode community, they have even taken some of the best things about Emacs and incorporated it, like [Magit](https://marketplace.visualstudio.com/items?itemName=kahole.magit) and an almost [Org Roam](https://vscode-org-mode.github.io/vscode-org-mode/) or [Custom second brain](https://hodgkins.io/vscode-second-brain).

So I scratched the itch and found that I could significantly improve performance using [MSYS2](https://www.msys2.org/). While this is based of Cygwin, it first natively compiles and leverages off Cygwin secondarily. 


### Steps

1. Install [MSYS2](https://www.msys2.org/)
1. Run "MSYS2 Msys", and install some key compilation requirements:
```
pacman -Sy --needed filesystem msys2-runtime bash libreadline libiconv libarchive libgpgme libcurl pacman ncurses libintl
```
1. Run the following, and then close and re-open.
```
pacman -Su
```
1. Install the Emacs dependencies
```
pacman -Su autoconf autogen automake automake-wrapper diffutils git guile libgc libguile libltdl libunistring  make mingw-w64-x86_64-binutils mingw-w64-x86_64-bzip2 mingw-w64-x86_64-cairo mingw-w64-x86_64-cloog mingw-w64-x86_64-crt-git mingw-w64-x86_64-dbus mingw-w64-x86_64-expat mingw-w64-x86_64-fontconfig mingw-w64-x86_64-freetype mingw-w64-x86_64-gcc mingw-w64-x86_64-gcc-libs mingw-w64-x86_64-gdk-pixbuf2 mingw-w64-x86_64-gettext mingw-w64-x86_64-giflib mingw-w64-x86_64-glib2 mingw-w64-x86_64-gmp mingw-w64-x86_64-gnutls mingw-w64-x86_64-harfbuzz mingw-w64-x86_64-headers-git mingw-w64-x86_64-imagemagick mingw-w64-x86_64-isl mingw-w64-x86_64-libcroco mingw-w64-x86_64-libffi mingw-w64-x86_64-libgccjit mingw-w64-x86_64-libiconv  mingw-w64-x86_64-libjpeg-turbo mingw-w64-x86_64-libpng mingw-w64-x86_64-librsvg mingw-w64-x86_64-libtiff mingw-w64-x86_64-libwinpthread-git mingw-w64-x86_64-libxml2 mingw-w64-x86_64-mpc mingw-w64-x86_64-mpfr mingw-w64-x86_64-pango mingw-w64-x86_64-pixman mingw-w64-x86_64-winpthreads mingw-w64-x86_64-xpm-nox mingw-w64-x86_64-lcms2 mingw-w64-x86_64-xz mingw-w64-x86_64-zlib tar wget
```
1. Clone Emacs 
```
git clone http://git.savannah.gnu.org/r/emacs.git emacs-29
```
1. Git Config requirement
```
git config core.autocrlf false
```
1. Setup
```
  cd emacs-29
  ./autogen.sh
```
1. Configure
Configure the features and functionality you may require. With the last argument 'prefix', you can define the location for your installation.
```
 ./configure     --host=x86_64-w64-mingw32     --target=x86_64-w64-mingw32     --build=x86_64-w64-mingw32     --with-native-compilation     --with-gnutls     --with-imagemagick     --with-jpeg     --with-json     --with-png     --with-rsvg     --with-tiff     --with-wide-int     --with-xft     --with-xml2     --with-xpm     'CFLAGS=-I/mingw64/include/noX'     prefix=/c/emacs/
```
1. Build and Install
```
make
make install prefix=/c/emacs/
```

### Success
Here on out you should have a successful build. It is from here I then install Doom Emacs. A key step I initially missed was setting an environment variable for the mingw64 libraries and binaries as a Windows environment variable.
```
C:\msys64\mingw64\bin
```

After Doom install, I use command prompt or Powershell to start Emacs
```
doom run
```

### Installation Options Pros and Cons
<a id="org4d72446"></a>

<table border="2" cellspacing="0" cellpadding="6" rules="groups" frame="hsides">


<colgroup>
<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-left" />

<col  class="org-right" />
</colgroup>
<tbody>
<tr>
<td class="org-left">Type of Install</td>
<td class="org-left">Peformant</td>
<td class="org-left">Native</td>
<td class="org-left">Image paste</td>
<td class="org-left">Broken dependencies</td>
<td class="org-left">Complex setup</td>
<td class="org-left">Networking issues</td>
<td class="org-left">Terminal</td>
<td class="org-left">Stability</td>
<td class="org-left">Host OS</td>
<td class="org-right">Version</td>
</tr>


<tr>
<td class="org-left">Windows Binary</td>
<td class="org-left">Slow</td>
<td class="org-left">Y</td>
<td class="org-left">Y</td>
<td class="org-left">N</td>
<td class="org-left">N</td>
<td class="org-left">N</td>
<td class="org-left">Eshell</td>
<td class="org-left">Very good</td>
<td class="org-left">Windows</td>
<td class="org-right">28</td>
</tr>


<tr>
<td class="org-left">Msys2 Binary</td>
<td class="org-left">Very fast</td>
<td class="org-left">Y</td>
<td class="org-left">Y</td>
<td class="org-left">Y (no java, dotnet)</td>
<td class="org-left">Y</td>
<td class="org-left">N</td>
<td class="org-left">Eshell</td>
<td class="org-left">Very good</td>
<td class="org-left">Msys2</td>
<td class="org-right">28</td>
</tr>


<tr>
<td class="org-left">Msys2 Source</td>
<td class="org-left">Very fast</td>
<td class="org-left">Y</td>
<td class="org-left">Y</td>
<td class="org-left">Y (no java, dotnet)</td>
<td class="org-left">Y</td>
<td class="org-left">N</td>
<td class="org-left">Eshell</td>
<td class="org-left">Good</td>
<td class="org-left">Msys2</td>
<td class="org-right">29</td>
</tr>


<tr>
<td class="org-left">WSL2</td>
<td class="org-left">Fast</td>
<td class="org-left">N</td>
<td class="org-left">N</td>
<td class="org-left">N</td>
<td class="org-left">Y</td>
<td class="org-left">Y</td>
<td class="org-left">VTerm</td>
<td class="org-left">Good</td>
<td class="org-left">Linux</td>
<td class="org-right">29</td>
</tr>
</tbody>
</table>

