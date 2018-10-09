# DEW_CSV

This console program will do the following:
1. Fetch CSV file from https://apps.waterconnect.sa.gov.au/file.csv
1. Delete the column with name _Unit_No_
1. Add the values in columns _swl_ and _rswl_ and put the sum in the new column named _calc_
1. Output the name and path of the newly generated file

Please note that this program does not handle newline in comments.
