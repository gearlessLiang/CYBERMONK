extends State
class_name PilgrimLeave

@export var Pilgrim: CharacterBody2D
@export var move_speed:= 0


var wait_time: float

var animationSprite
#
#var path_stone_1_attend: bool = false
#var path_stone_2_attend: bool = false

var path_stone_pool: Array
var path_stone
var path_stone_num: int = 0

func _ready():
	animationSprite = get_node("../../Sprite")
	
	path_stone = get_node("../../../PathStoneLeave")
	for i in path_stone.get_children():
		path_stone_pool.append(i)
		

		
	
func enter():
	pass


func physics_update(delta: float):
	if path_stone_num < path_stone_pool.size():
		Pilgrim.global_position += (path_stone_pool[path_stone_num].global_position - Pilgrim.global_position).normalized() 
		

		if Pilgrim.global_position.distance_to((path_stone_pool[path_stone_num].global_position)) < 2:
			path_stone_num += 1
			flip_check()

	#get_node("../..").global_position += -(get_node("../..").global_position).normalized()
	
func flip_check():
	if path_stone_num < path_stone_pool.size():
		#面向方向的改变
		if Pilgrim.global_position > path_stone_pool[path_stone_num].global_position:
			Pilgrim.get_node("Sprite").flip_h = true
		else:
			Pilgrim.get_node("Sprite").flip_h = false

	
	
	
func exist():
	pass



func choose_target():
	pass
		
