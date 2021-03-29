# AFRollupDR

A simple datareference that performs rollup-calculations on the attributes of an AF-element with the same category.

# AFBitmaskDR

Datareference, which performs bit extraction from a SCADA-tag containing a bit set. The bits are extracted into attributes with this datareference, according to the specified AF-attribute of the data source and the number of the desired bit.

# AFAttrLookupDR

# AFTransformerDR

# AFCalibLookupDR (AF Calibration Lookup DataReference)

A data reference that lookups for values using a calibration table.

# Debugging

For correct debugging of any CDRs, it is extremely important to change the version number of assembly 
after each group of changes - in order to correctly update the AF plugin cache (ProgramData\OSIsoft\AF\PlugIns).
