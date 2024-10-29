case $NUNIT_VERSION in 
  3.7.*|3.8.*|3.9.*) 
      forced="netstandard2.0;net462"
      TARGET_FRAMEWORKS_LIB="netstandard1.3;netstandard1.6;net20;net35;net40;net45;$forced"
      TARGET_FRAMEWORKS_TEST="netcoreapp1.0;netcoreapp1.1;netcoreapp2.0;netcoreapp2.1;netcoreapp3.0;netcoreapp3.1;net20;net35;net40;net45;net462;net48;net8.0"
      ;;
  3.10.*) 
      # the latest net20
      forced="net462"
      TARGET_FRAMEWORKS_LIB="netstandard1.6;netstandard2.0;net20;net35;net4.0;net45;$forced"
      TARGET_FRAMEWORKS_TEST="netcoreapp1.0;netcoreapp1.1;netcoreapp2.0;netcoreapp2.1;netcoreapp3.0;netcoreapp3.1;net20;net35;net40;net45;net462;net48;net8.0"
      ;;
  3.11.*) 
      # the latest net20
      forced="net462;netstandard1.6"
      TARGET_FRAMEWORKS_LIB="netstandard1.4;netstandard2.0;net20;net35;net4.0;net45;$forced"
      TARGET_FRAMEWORKS_TEST="netcoreapp1.0;netcoreapp1.1;netcoreapp2.0;netcoreapp2.1;netcoreapp3.0;netcoreapp3.1;net20;net35;net40;net45;net462;net48;net8.0"
      ;;
  3*) 
      forced="net462"
      TARGET_FRAMEWORKS_LIB="netstandard2.0;net35;net40;net45;$forced"
      TARGET_FRAMEWORKS_TEST="net8.0;net6.0;netcoreapp3.1;net48;net462;net45;net40;net35;net8.0"
      ;;
  *)
      # v4.x
      TARGET_FRAMEWORKS_LIB="net462;net6.0";
      TARGET_FRAMEWORKS_TEST="net8.0;net6.0;net48;net462";
      ;;
esac
