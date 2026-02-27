extends State
class_name MonkIdle

@export var Monk: CharacterBody2D
@export var move_speed:= 0


var wait_time: float

var animationSprite


func random_wait_time():
	wait_time = randf_range(1,3)

func _ready():
	animationSprite = get_node("../../Sprite")

#
#func lock_trace():
	#pass
	
func enter():
	play_muyu()
	random_wait_time()
	pass

func play_muyu():
	animationSprite.play("default")


func physics_update(delta: float):
	if wait_time > 0:
		wait_time -= delta

	else:
		transitioned.emit(self, "monkpatrol")

	
	
func exist():
	pass



func choose_target():
	pass
		
