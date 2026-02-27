extends State
class_name MonkPatrol

@export var Monk: CharacterBody2D
@export var move_speed:= 20

var move_to_right: bool
var wander_time: float
var animationSprite

func random_wander():
	move_to_right = randf() > 0.5
	wander_time = randf_range(2, 5)
	
	if move_to_right == true:
		animationSprite.flip_h = false
	else:
		animationSprite.flip_h = true

func _ready():
	animationSprite = get_node("../../Sprite")
	choose_target()

	
func enter():
	random_wander()
	animationSprite.play("walk")
	

func physics_update(delta: float):
	if wander_time > 0:
		wander_time -= delta
		if move_to_right == true:	
			get_node("../..").position.x += 1
		else:
			get_node("../..").position.x -= 1
	else:
		transitioned.emit(self, "monkidle")
	
	
func exist():
	pass
	
func choose_target():
	pass
		
		
		
