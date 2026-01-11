# SNIL Commands Documentation

### Clear Background
```snil
Clear Background
```

### Compare Integers
```snil
Compare {a} {type} {b}
```

### Debug
```snil
Print {message}
```

### Dialog
```snil
{_character} says {_text}
```

### Dialog On Screen
```snil
Dialog On Screen {_text} 
```

### Error
```snil
Error {message}
```

### Exit
```snil
End
```

### Group Calls
```snil
function {_name}
```

### Hide Character
```snil
Hide {_character}
```

### If
```snil
IF {condition}
```

### Jump To Dialogue
```snil
Jump To {_dialogue}
```

### Play Sound
```snil
Play Sound {_sound} 
```

### Set Background
```snil
Show Background {_sprite}
```

### Show Character
```snil
Show {_character} with emotion {_emotion}
```

### Show Variants
```snil
Show Variants {_variants}
```

### Start
```snil
Start
```

### Wait
```snil
Wait {seconds} seconds
```

### If Show Variant
```snil
If Show Variant
Variants:
[Option A]
[Option B]
...
True:
[Commands for selected variant]
False:
[Commands if no variant matched or cancelled]
endif
```

### Switch Show Variant
```snil
Switch Show Variant
Cases:
Case [value]:
[Commands for this case]
Case [value]:
[Commands for this case]
[Additional cases...]
endcase
```

### Create Variable or Set
```snil
set [variable_name] = [value]
```

