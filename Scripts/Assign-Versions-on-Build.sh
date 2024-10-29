case $NUNIT_VERSION in 
  3.7.*|3.8.*) 
      TARGET_FRAMEWORKS_LIB="netstandard1.3;netstandard1.6;net20;net35;net45; netstandard2.0;net462"
      TARGET_FRAMEWORKS_TEST="netcoreapp1.0;netcoreapp1.1;netcoreapp2.0;netcoreapp2.1;netcoreapp3.0;netcoreapp3.1;net20;net35;net45;net462;net48"
      ;;
  3*) 
      TARGET_FRAMEWORKS_LIB="netstandard2.0;net35;net40;net45; netstandard2.0;net462"
      TARGET_FRAMEWORKS_TEST="net8.0;net6.0;netcoreapp3.1;net48;net462;net45;net40;net35"
      ;;
  *)
      TARGET_FRAMEWORKS_LIB="net462;net6.0";
      TARGET_FRAMEWORKS_TEST="net8.0;net6.0;net48;net462";
      ;;
esac
