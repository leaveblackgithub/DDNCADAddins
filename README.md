# DDNCADAddinsForRevitImport

In development...

# Issues
When importing/linking CAD into Revit, usually encounting these problems:
1. Different linestyles/colors on same layers are difficult to show correctly
2. Xcliped blocks will be reverted and shown incorrectly.
3. Need to split plans to different drawings.
4. Sometimes polylines are used to show column hatch.
5. Some hatches may leak.

# Target
1. Cleanup layers. Features as following:
  a. Merge layers with xref prefix
  b. Create new layers for different linestyles/colors(optional)
  c. Make objects linestyles/colors "Bylayer"
  d. Audit and purge
2. Convert all xclip to crop
