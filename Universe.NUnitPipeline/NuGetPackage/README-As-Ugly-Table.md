﻿| Test                                                          | Status       | Duration | CPU (%) | CPU (ms) | User   | Kernel | Message                                                 |
| ------------------------------------------------------------- | ------------ | -------- | ------- | -------- | ------ | ------ | ------------------------------------------------------- |
| Tests                                                         |              |          |         |          |        |        |                                                         |
|  ├── TestsAsync (fixture)                                     |              |          |         |          |        |        |                                                         |
|  │   ├── AsyncException                                       |              |          |         |          |        |        |                                                         |
|  │   │   ├── AsyncException("First",milliseconds=7)           | Failed:Error |    29.00 |  107.87 |    31.26 |  26.20 |   5.06 | System.InvalidOperationException : Exception on purpose |
|  │   │   └── AsyncException("Next",milliseconds=200)          | Failed:Error |   201.20 |   99.98 |   201.17 | 135.33 |  65.84 | System.InvalidOperationException : Exception on purpose |
|  │   ├── AsyncFail                                            |              |          |         |          |        |        |                                                         |
|  │   │   ├── AsyncFail("First",milliseconds=7)                | Failed       |    28.00 |  104.57 |    29.31 |  25.03 |   4.29 | Fail on purpose                                         |
|  │   │   └── AsyncFail("Next",milliseconds=200)               | Failed       |   201.30 |  100.08 |   201.45 | 135.64 |  65.81 | Fail on purpose                                         |
|  │   └── AsyncSuccess                                         |              |          |         |          |        |        |                                                         |
|  │       ├── AsyncSuccess("First",milliseconds=7)             | PASSED       |     8.30 |  108.64 |     8.98 |   6.41 |   2.58 |                                                         |
|  │       └── AsyncSuccess("Next",milliseconds=200)            | PASSED       |   200.60 |  100.09 |   200.77 | 136.57 |  64.20 |                                                         |
|  ├── TestsOverThreadPool (fixture)                            |              |          |         |          |        |        |                                                         |
|  │   ├── ExceptionOverThreadPool                              |              |          |         |          |        |        |                                                         |
|  │   │   ├── ExceptionOverThreadPool("First",milliseconds=7)  | Failed:Error |     8.30 |  103.10 |     8.57 |   5.92 |   2.65 | System.InvalidOperationException : Exception on purpose |
|  │   │   └── ExceptionOverThreadPool("Next",milliseconds=200) | Failed:Error |   201.20 |  100.10 |   201.38 | 136.80 |  64.58 | System.InvalidOperationException : Exception on purpose |
|  │   ├── FailOverThreadPoolFail                               |              |          |         |          |        |        |                                                         |
|  │   │   ├── FailOverThreadPoolFail("First",milliseconds=7)   | Failed       |     8.30 |  102.32 |     8.50 |   5.92 |   2.58 | Fail on purpose                                         |
|  │   │   └── FailOverThreadPoolFail("Next",milliseconds=200)  | Failed       |   201.10 |  100.13 |   201.35 | 136.74 |  64.61 | Fail on purpose                                         |
|  │   └── SuccessOverThreadPool                                |              |          |         |          |        |        |                                                         |
|  │       ├── SuccessOverThreadPool("First",milliseconds=7)    | PASSED       |     7.70 |   99.54 |     7.68 |   5.32 |   2.36 |                                                         |
|  │       └── SuccessOverThreadPool("Next",milliseconds=200)   | PASSED       |   200.70 |  100.08 |   200.83 | 136.39 |  64.44 |                                                         |
|  └── TestsSync (fixture)                                      |              |          |         |          |        |        |                                                         |
|      ├── ExceptionSynchronously                               |              |          |         |          |        |        |                                                         |
|      │   ├── ExceptionSynchronously("First",milliseconds=7)   | Failed:Error |     8.20 |  100.00 |     8.19 |   5.85 |   2.34 | System.InvalidOperationException : Exception on purpose |
|      │   └── ExceptionSynchronously("Next",milliseconds=200)  | Failed:Error |   204.20 |   98.52 |   201.20 | 137.80 |  63.40 | System.InvalidOperationException : Exception on purpose |
|      ├── FailSynchronously                                    |              |          |         |          |        |        |                                                         |
|      │   ├── FailSynchronously("First",milliseconds=7)        | Failed       |     8.10 |  100.05 |     8.09 |   5.81 |   2.28 | Fail on purpose                                         |
|      │   └── FailSynchronously("Next",milliseconds=200)       | Failed       |   201.00 |   99.95 |   200.90 | 135.24 |  65.66 | Fail on purpose                                         |
|      └── SuccessSynchronously                                 |              |          |         |          |        |        |                                                         |
|          ├── SuccessSynchronously("First",milliseconds=7)     | PASSED       |     7.30 |  100.02 |     7.32 |   5.07 |   2.25 |                                                         |
|          └── SuccessSynchronously("Next",milliseconds=200)    | PASSED       |   200.50 |   99.96 |   200.40 | 135.27 |  65.13 |                                                         |
  