extends State
class_name PilgrimWait

@export var Pilgrim: CharacterBody2D
@export var move_speed:= 0


var wait_time: float


func _ready():
	pass
		
	
func enter():
	pass


func physics_update(delta: float):
	Pilgrim.scale *= 1.5
	transitioned.emit(self, "PilgrimLeave")
	#get_node("../..").global_position += -(get_node("../..").global_position).normalized()

	
	
func exist():
	pass

func choose_target():
	pass
		
