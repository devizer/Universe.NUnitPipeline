versions='
4.2.2
4.2.1
4.1.0
4.0.1
3.14.0
3.13.3
3.13.2
3.13.1
3.13.0
3.12.0
3.11.0
3.10.1
3.10.0
3.9.0
3.8.1
3.8.0
3.7.1
3.7.0
3.6.1
3.6.0
3.5.0
3.4.1
3.4.0
3.2.1
3.2.0
3.0.1
3.0.0
'
for v in $versions; do
echo '  "'$v' Windows":
     NUNIT_VERSION: "'$v'"
     POOL: "windows-latest"
  "'$v' Linux":
     NUNIT_VERSION: "'$v'"
     POOL: "ubuntu-latest"
  "'$v' MacOs":
     NUNIT_VERSION: "'$v'"
     POOL: "macos-latest"'

done
