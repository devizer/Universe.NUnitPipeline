### Universe.NUnitPipeline 
Customizable NUnit pipeline for 3.x and 4.x+ NUnit. The main purpose is to measure and report CPU usage. 

![CPU Usage Tree Report](https://raw.githubusercontent.com/devizer/Universe.NUnitPipeline/main/Universe.NUnitPipeline/NuGetPackage/Cpu-Usage-Screenshot-V5.png)

CPU Usage is properly measured for cpu load by:
- Task.Run(), Task.Factory.StartNew(), and await
- ThreadPool.QueueUserWorkItem();
- Thread.Start()


### Supported OS
It supports Windows (Vista+, x86, x64, and arm), Linux (2.6.26+), and Mac OS (10.9+). 
WSL 1+, Termux, and mono-only platforms are also supported.


### Supported Target Frameworks
The target frameworks are derived from the corresponding NUnit version:
- 4.x: Net Framework 4.6.2+, and Net Core 6.0+
- 3.12 ... 3.14: Net Framework 3.5+, and Net Standard 2.0+
- 3.7 ... 3.10: Net Framework 2.0+, Net Core 1.0+, and Net Standard 1.x+
