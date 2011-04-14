//By Jason Zelsnack, All rights reserved

using System;

namespace NovodexWrapper
{
	//This is my own enumeration
	public enum MatrixAxis{X=0,Y=1,Z=2,Pos}

	public enum NxParameter
	{
		NX_PENALTY_FORCE,					//!< DEPRECATED! Do not use!
		NX_SKIN_WIDTH,						//!< Default value for ::NxShapeDesc::skinWidth, see for more info.  (range: [0, inf]) Default: 0.05, Unit: distance.
		NX_DEFAULT_SLEEP_LIN_VEL_SQUARED,	//!< The default linear velocity, squared, below which objects start going to sleep. (range: [0, inf]) Default: (0.15*0.15)
		NX_DEFAULT_SLEEP_ANG_VEL_SQUARED,	//!< The default angular velocity, squared, below which objects start going to sleep. (range: [0, inf]) Default: (0.14*0.14)
		NX_BOUNCE_THRESHOLD,				//!< A contact with a relative velocity below this will not bounce.	(range: [-inf, 0]) Default: -2
		NX_DYN_FRICT_SCALING,				//!< This lets the user scale the magnitude of the dynamic friction applied to all objects.	(range: [0, inf]) Default: 1
		NX_STA_FRICT_SCALING,				//!< This lets the user scale the magnitude of the static friction applied to all objects.	(range: [0, inf]) Default: 1
		NX_MAX_ANGULAR_VELOCITY,			//!< See the comment for NxBody::setMaxAngularVelocity() for details.	Default: 7
		NX_CONTINUOUS_CD,					//!< Enable/disable continuous collision detection (0.0f to disable)
		NX_VISUALIZATION_SCALE,				//!< This overall visualization scale gets multiplied with the individual scales. Setting to zero turns ignores all visualizations. Default is 0.
		NX_VISUALIZE_WORLD_AXES,
		NX_VISUALIZE_BODY_AXES,
		NX_VISUALIZE_BODY_MASS_AXES,
		NX_VISUALIZE_BODY_LIN_VELOCITY,
		NX_VISUALIZE_BODY_ANG_VELOCITY,
		NX_VISUALIZE_BODY_LIN_MOMENTUM,
		NX_VISUALIZE_BODY_ANG_MOMENTUM,
		NX_VISUALIZE_BODY_LIN_ACCEL,
		NX_VISUALIZE_BODY_ANG_ACCEL,
		NX_VISUALIZE_BODY_LIN_FORCE,
		NX_VISUALIZE_BODY_ANG_FORCE,
		NX_VISUALIZE_BODY_REDUCED,
		NX_VISUALIZE_BODY_JOINT_GROUPS,
		NX_VISUALIZE_BODY_CONTACT_LIST,
		NX_VISUALIZE_BODY_JOINT_LIST,
		NX_VISUALIZE_BODY_DAMPING,
		NX_VISUALIZE_BODY_SLEEP,
		NX_VISUALIZE_JOINT_LOCAL_AXES,
		NX_VISUALIZE_JOINT_WORLD_AXES,
		NX_VISUALIZE_JOINT_LIMITS,
		NX_VISUALIZE_JOINT_ERROR,
		NX_VISUALIZE_JOINT_FORCE,
		NX_VISUALIZE_JOINT_REDUCED,
		NX_VISUALIZE_CONTACT_POINT,
		NX_VISUALIZE_CONTACT_NORMAL,
		NX_VISUALIZE_CONTACT_ERROR,
		NX_VISUALIZE_CONTACT_FORCE,
		NX_VISUALIZE_ACTOR_AXES,
		NX_VISUALIZE_COLLISION_AABBS,
		NX_VISUALIZE_COLLISION_SHAPES,
		NX_VISUALIZE_COLLISION_AXES,
		NX_VISUALIZE_COLLISION_COMPOUNDS,
		NX_VISUALIZE_COLLISION_VNORMALS,
		NX_VISUALIZE_COLLISION_FNORMALS,
		NX_VISUALIZE_COLLISION_EDGES,
		NX_VISUALIZE_COLLISION_SPHERES,
		NX_VISUALIZE_COLLISION_SAP,
		NX_VISUALIZE_COLLISION_STATIC,
		NX_VISUALIZE_COLLISION_DYNAMIC,
		NX_VISUALIZE_COLLISION_FREE,
		NX_VISUALIZE_COLLISION_CCD,	
		NX_VISUALIZE_COLLISION_SKELETONS,
		NX_VISUALIZE_FLUID_EMITTERS,
		NX_VISUALIZE_FLUID_POSITION,
		NX_VISUALIZE_FLUID_VELOCITY,
		NX_VISUALIZE_FLUID_KERNEL_RADIUS,
		NX_VISUALIZE_FLUID_BOUNDS,
		NX_VISUALIZE_FLUID_PACKETS,
		NX_VISUALIZE_FLUID_MOTION_LIMIT,
		NX_VISUALIZE_FLUID_DYN_COLLISION,
		NX_VISUALIZE_FLUID_DRAINS,
		NX_VISUALIZE_CLOTH_COLLISIONS,
		NX_VISUALIZE_CLOTH_SELFCOLLISIONS,
		NX_VISUALIZE_CLOTH_WORKPACKETS,
		NX_ADAPTIVE_FORCE,
		NX_COLL_VETO_JOINTED,
		NX_TRIGGER_TRIGGER_CALLBACK,
		NX_SELECT_HW_ALGO,
		NX_VISUALIZE_ACTIVE_VERTICES,
		NX_CCD_EPSILON,
		NX_SOLVER_CONVERGENCE_THRESHOLD,
		NX_BBOX_NOISE_LEVEL,
		NX_PARAMS_NUM_VALUES,				//!< This is not a parameter, it just records the current number of parameters.
		NX_MIN_SEPARATION_FOR_PENALTY		//!< Deprecated! Use SKIN_WIDTH instead.  The minimum contact separation value in order to apply a penalty force. (range: [-inf, 0) ) I.e. This must be negative!  Default: -0.05  Note: its OK for this one to be > than NX_PARAMS_NUM_VALUES because internally it uses the slot of SKIN_WIDTH.
	}

	public enum NxTimeStepMethod
	{
		NX_TIMESTEP_FIXED,				//!< The simulation automatically subdivides the passed elapsed time into maxTimeStep-sized substeps.
		NX_TIMESTEP_VARIABLE,			//!< The simulation uses the elapsed time that the user passes as-is, substeps (maxTimeStep, maxIter) are not used.
		NX_NUM_TIMESTEP_METHODS,
	}

	public enum NxSceneFlags
	{
		NX_SF_DISABLE_SSE				= 0x1,
		NX_SF_DISABLE_COLLISIONS		= 0x2,
		NX_SF_SIMULATE_SEPARATE_THREAD	= 0x4,
		NX_SF_ENABLE_MULTITHREAD		= 0x8
	};

	public enum NxHwPipelineSpec
	{
		NX_HW_RB_PIPELINE_HLP_ONLY  = 0,
		NX_HW_PIPELINE_FULL         = 1,
		NX_HW_PIPELINE_DEBUG        = 2
	}

	public enum NxHwSceneType
	{
		NX_HW_SCENE_TYPE_RB				= 0,	//!< Specifies a rigid body hardware scene.
		NX_HW_SCENE_TYPE_FLUID			= 1,	//!< Specifies a fluid hardware scene.
		NX_HW_SCENE_TYPE_FLUID_SOFTWARE	= 2,	//!< Specifies a scene, running the fluid software reference. 
		NX_HW_SCENE_TYPE_CLOTH			= 3		//!< Specifies a cloth hardware scene.
	}

	public enum NxSimulationType
	{
		NX_SIMULATION_SW	= 0,
		NX_SIMULATION_HW	= 1
	}
	
	public enum NxAxisType
	{
		NX_AXIS_PLUS_X,
		NX_AXIS_MINUS_X,
		NX_AXIS_PLUS_Y,
		NX_AXIS_MINUS_Y,
		NX_AXIS_PLUS_Z,
		NX_AXIS_MINUS_Z,
		NX_AXIS_ARBITRARY
	}

	public enum NxForceMode
	{
		NX_FORCE,                   //!< parameter has unit of mass * distance/ time^2, i.e. a force
		NX_IMPULSE,                 //!< parameter has unit of mass * distance /time
		NX_VELOCITY_CHANGE,			//!< parameter has unit of distance / time, i.e. the effect is mass independent: a velocity change.
		NX_SMOOTH_IMPULSE,          //!< same as NX_IMPULSE but the effect is applied over all substeps.  Use this for motion controllers that repeatedly apply an impulse.
		NX_SMOOTH_VELOCITY_CHANGE,	//!< same as NX_VELOCITY_CHANGE but the effect is applied over all substeps.  Use this for motion controllers that repeatedly apply an impulse.
		NX_ACCELERATION				//!< parameter has unit of distance/ time^2, i.e. an acceleration.  It gets treated just like a force except the mass is not divided out before integration.
	}

	public enum NxMaterialFlag
	{
		NX_MF_ANISOTROPIC = 1 << 0,
		NX_MF_SPRING_CONTACT = 1 << 2,
		NX_MF_DISABLE_FRICTION = 1 << 4,
		NX_MF_DISABLE_STRONG_FRICTION = 1 << 5,
		NX_MF_WHEEL_AXIS_CONTACT_NORMAL = 1 << 6
	}
	
	public enum NxCombineMode : uint
	{
		NX_CM_AVERAGE = 0,
		NX_CM_MIN = 1,
		NX_CM_MULTIPLY = 2,
		NX_CM_MAX = 3,
		NX_CM_N_VALUES = 4,	//this a sentinel to denote the number of possible values. We assert that the variable's value is smaller than this.
		NX_CM_PAD_32 = 0xffffffff 
	}

	public enum NxStandardFences
	{
		NX_FENCE_RUN_FINISHED,
		NX_NUM_STANDARD_FENCES
	}

	public enum NxSimulationStatus
	{
		NX_RIGID_BODY_FINISHED	= (1<<0)
	}

	public enum NxActorFlag
	{
		NX_AF_DISABLE_COLLISION			= (1<<0),	//!< Enable/disable collision detection
		NX_AF_DISABLE_RESPONSE			= (1<<1),	//!< Enable/disable collision response (reports contacts but don't use them) 
		NX_AF_LOCK_COM					= (1<<2),	//When sdk computes inertial properties, by default the center of mass will be calculated too.  However, if lockCOM is set to a non-zero (true) value then the center of mass will not be altered.
		NX_AF_FLUID_DISABLE_COLLISION	= (1<<3),	//!< Not available in the current release. Disable collision with fluids
		NX_AF_FLUID_ACTOR_REACTION		= (1<<4)	//!< Not available in the current release. Enable the reaction on fluid collision
	}

	public enum NxActorDescType
	{
		NX_ADT_SHAPELESS,
		NX_ADT_DEFAULT,
		NX_ADT_ALLOCATOR,
		NX_ADT_LIST,
		NX_ADT_POINTER
	}

	public enum NxAssertResponse
	{
		NX_AR_CONTINUE,			//!continue execution
		NX_AR_IGNORE,			//!continue and don't report this assert from now on
		NX_AR_BREAKPOINT		//!trigger a breakpoint
	}

	public enum NxErrorCode
	{
		NXE_NO_ERROR			= 0,	//no error
		NXE_INVALID_PARAMETER	= 1,	//method called with invalid parameter(s)
		NXE_INVALID_OPERATION	= 2,	//method was called at a time when an operation is not possible
		NXE_OUT_OF_MEMORY		= 3,	//method failed to allocate some memory
		NXE_INTERNAL_ERROR		= 4,	//the library failed for some reason (usually because you have passed invalid values like NaNs into the system, which are not checked for.)
		NXE_ASSERTION			= 107,	//an assertion failed.
		//messages only emitted when NX_USER_DEBUG_MODE is defined:
		NXE_DB_INFO				= 205,	//an information message for the user to help with debugging
		NXE_DB_WARNING			= 206,	//a warning message for the user to help with debugging
		NXE_DB_PRINT			= 208	//the message should simply be printed without any additional infos (line number, etc)
	}

	public enum NxMatrixType
	{
		NX_ZERO_MATRIX,
		NX_IDENTITY_MATRIX
	}

	public enum NxBodyFlag
	{
		NX_BF_DISABLE_GRAVITY	= (1<<0),	//!< set if gravity should not be applied on this body
		NX_BF_FROZEN_POS_X		= (1<<1),
		NX_BF_FROZEN_POS_Y		= (1<<2),
		NX_BF_FROZEN_POS_Z		= (1<<3),
		NX_BF_FROZEN_ROT_X		= (1<<4),
		NX_BF_FROZEN_ROT_Y		= (1<<5),
		NX_BF_FROZEN_ROT_Z		= (1<<6),
		NX_BF_FROZEN_POS		= NX_BF_FROZEN_POS_X|NX_BF_FROZEN_POS_Y|NX_BF_FROZEN_POS_Z,
		NX_BF_FROZEN_ROT		= NX_BF_FROZEN_ROT_X|NX_BF_FROZEN_ROT_Y|NX_BF_FROZEN_ROT_Z,
		NX_BF_FROZEN			= NX_BF_FROZEN_POS|NX_BF_FROZEN_ROT,
		NX_BF_KINEMATIC			= (1<<7),		//!< Enable kinematic mode for the body.
		NX_BF_VISUALIZATION		= (1<<8),		//!< Enable debug renderer for this body
		NX_BF_POSE_SLEEP_TEST	= (1<<9),
		NX_BF_FILTER_SLEEP_VEL	= (1<<10),
	}

	public enum NxShapeType : uint
	{
		NX_SHAPE_PLANE,
		NX_SHAPE_SPHERE,
		NX_SHAPE_BOX,
		NX_SHAPE_CAPSULE,
		NX_SHAPE_WHEEL,
		NX_SHAPE_CONVEX,
		NX_SHAPE_MESH,
		NX_SHAPE_HEIGHTFIELD,
		NX_SHAPE_RAW_MESH,	//Internal use only
		NX_SHAPE_COMPOUND,	//Internal use only
		NX_SHAPE_COUNT	//Internal use only
	};




	public enum NxShapeFlag
	{
		NX_TRIGGER_ON_ENTER				= (1<<0),	//!< Trigger callback will be called when a shape enters the trigger volume.
		NX_TRIGGER_ON_LEAVE				= (1<<1),	//!< Trigger callback will be called after a shape leaves the trigger volume.
		NX_TRIGGER_ON_STAY				= (1<<2),	//!< Trigger callback will be called while a shape is intersecting the trigger volume.
		NX_TRIGGER_ENABLE				= NX_TRIGGER_ON_ENTER|NX_TRIGGER_ON_LEAVE|NX_TRIGGER_ON_STAY,
		NX_SF_VISUALIZATION				= (1<<3),	//!< Enable debug renderer for this shape
		NX_SF_DISABLE_COLLISION			= (1<<4),	//!< Disable collision detection for this shape (counterpart of NX_AF_DISABLE_COLLISION)
		//!< IMPORTANT: this is only used for compound objects! Use NX_AF_DISABLE_COLLISION otherwise.
		NX_SF_FEATURE_INDICES			= (1<<5),	//!< Enable feature indices in contact stream.
		NX_SF_DISABLE_RAYCASTING		= (1<<6),	//!< Disable raycasting for this shape
		NX_SF_POINT_CONTACT_FORCE		= (1<<7),	//!< Enable contact force reporting per contact point in contact stream (otherwise we only report force per actor pair)
		NX_SF_FLUID_DRAIN				= (1<<8),	//!< Not available in the current release. Sets the shape to be a fluid drain.
		NX_SF_FLUID_DRAIN_INVERT		= (1<<9),	//!< Not available in the current release. Invert the domain of the fluid drain.
		NX_SF_FLUID_DISABLE_COLLISION	= (1<<10),	//!< Not available in the current release. Disable collision with fluids.
		NX_SF_FLUID_ACTOR_REACTION		= (1<<11),	//!< Not available in the current release. Enables the reaction of the shapes actor on fluid collision.
		NX_SF_DISABLE_RESPONSE			= (1<<12),	//!< Disable collision response for this shape (counterpart of NX_AF_DISABLE_RESPONSE)
		NX_SF_DYNAMIC_DYNAMIC_CCD		= (1<<13),  //Enable dynamic-dynamic CCD for this shape. Used only when CCD is globally enabled and shape have a CCD skeleton.
		USE_DEFAULT			//this is mine
	}

	public enum NxJointFlag
	{
		NX_JF_COLLISION_ENABLED	= (1<<0),	//!< Raised if collision detection should be enabled between the jointed parts.
		NX_JF_VISUALIZATION		= (1<<1),	//!< Enable debug renderer for this joint
	}

	public enum NxJointProjectionMode
	{
		NX_JPM_NONE  = 0,				//!< don't project this joint
		NX_JPM_POINT_MINDIST = 1,		//!< this is the only projection method right now 
	}

	public enum NxJointType : uint
	{
		NX_JOINT_PRISMATIC,			//!< Permits a single translational degree of freedom.
		NX_JOINT_REVOLUTE,			//!< Also known as a hinge joint, permits one rotational degree of freedom.
		NX_JOINT_CYLINDRICAL,		//!< Formerly known as a sliding joint, permits one translational and one rotational degree of freedom.
		NX_JOINT_SPHERICAL,			//!< Also known as a ball or ball and socket joint.
		NX_JOINT_POINT_ON_LINE,		//!< A point on one actor is constrained to stay on a line on another.
		NX_JOINT_POINT_IN_PLANE,	//!< A point on one actor is constrained to stay on a plane on another.
		NX_JOINT_DISTANCE,			//!< A point on one actor maintains a certain distance range to another point on another actor.
		NX_JOINT_PULLEY,			//!< A pulley joint.
		NX_JOINT_FIXED,				//!< A "fixed" connection.
		NX_JOINT_D6,				//!< A 6 degree of freedom joint
		NX_JOINT_COUNT,				//!< Just to track the number of available enum values. Not a joint type.
	}

	public enum NxJointState
	{
		NX_JS_UNBOUND,
		NX_JS_SIMULATING,
		NX_JS_BROKEN
	}

	public enum NxSphericalJointFlag
	{
		NX_SJF_TWIST_LIMIT_ENABLED = 1 << 0,//!< true if the twist limit is enabled
		NX_SJF_SWING_LIMIT_ENABLED = 1 << 1,//!< true if the swing limit is enabled
		NX_SJF_TWIST_SPRING_ENABLED= 1 << 2,//!< true if the twist spring is enabled
		NX_SJF_SWING_SPRING_ENABLED= 1 << 3,//!< true if the swing spring is enabled
		NX_SJF_JOINT_SPRING_ENABLED= 1 << 4,//!< true if the joint spring is enabled
	}

	public enum NxRevoluteJointFlag
	{
		NX_RJF_LIMIT_ENABLED  = 1 << 0,			//!< true if the limit is enabled
		NX_RJF_MOTOR_ENABLED  = 1 << 1,			//!< true if the motor is enabled
		NX_RJF_SPRING_ENABLED = 1 << 2,			//!< true if the spring is enabled.  The spring will only take effect if the motor is disabled.
	}

	public enum NxDistanceJointFlag
	{
		NX_DJF_MAX_DISTANCE_ENABLED = 1 << 0,	//!< true if the joint enforces the maximum separate distance.
		NX_DJF_MIN_DISTANCE_ENABLED = 1 << 1,	//!< true if the joint enforces the minimum separate distance.
		NX_DJF_SPRING_ENABLED		= 1 << 2,	//!< true if the spring is enabled
	}

	public enum NxCapsuleShapeFlag
	{
		NX_SWEPT_SHAPE	= (1<<0),	//If this flag is set, the capsule shape represents a moving sphere, moving along the ray defined by the capsule's positive Y axis. Currently this behavior is only implemented for points (zero radius spheres).
	}

	public enum NxPulleyJointFlag
	{
		NX_PJF_IS_RIGID = 1 << 0,		//!< true if the joint also maintains a minimum distance, not just a maximum.
		NX_PJF_MOTOR_ENABLED = 1 << 1	//!< true if the motor is enabled
	}

	public enum NxD6JointMotion
	{
		NX_D6JOINT_MOTION_LOCKED,
		NX_D6JOINT_MOTION_LIMITED,
		NX_D6JOINT_MOTION_FREE
	}

	public enum NxD6JointLockFlags
	{
		NX_D6JOINT_LOCK_X			= 1<<0,		//!< Constrain relative motion in the X axis.
		NX_D6JOINT_LOCK_Y			= 1<<1,		//!< Constrain relative motion in the Y axis.
		NX_D6JOINT_LOCK_Z			= 1<<2,		//!< Constrain relative motion in the Z axis.
		NX_D6JOINT_LOCK_LINEAR		= 7,		//!<  NX_D6JOINT_LOCK_X | NX_D6JOINT_LOCK_Y | NX_D6JOINT_LOCK_Z ie apply to all linear motion.
		NX_D6JOINT_LOCK_TWIST		= 1<<3,		//!< Constrain twist motion(ie rotation around the joints axis)
		NX_D6JOINT_LOCK_SWING1		= 1<<4,		//!< Constrain swing motion(ie rotation around the joints normal axis)
		NX_D6JOINT_LOCK_SWING2		= 1<<5,		//!< Constrain swing motion(ie rotation around the joints binormal axis)
		NX_D6JOINT_LOCK_ANGULAR		= 7<<3,		//!< NX_D6JOINT_LOCK_TWIST | NX_D6JOINT_LOCK_SWING1 | NX_D6JOINT_LOCK_SWING2 ie apply to all angular motion.
	}
	
	public enum NxD6JointLimitFlags
	{
		NX_D6JOINT_LIMIT_TWIST	= 1<<0,			//!< Apply a limit to the joints twist(ie rotation around the joint axis)
		NX_D6JOINT_LIMIT_SWING	= 1<<1,			//!< Apply limits to the joints swing axis(ie rotation around the joints normal and binormal axis)
		NX_D6JOINT_LIMIT_LINEAR	= 1<<2			//!< Apply limits to the joints Linear motion.
	}

	public enum NxD6JointDriveType
	{
		NX_D6JOINT_DRIVE_POSITION	= 1<<0,
		NX_D6JOINT_DRIVE_VELOCITY	= 1<<1
	}

	public enum NxD6JointFlag
	{
		NX_D6JOINT_SLERP_DRIVE = 1<<0,
		NX_D6JOINT_GEAR_ENABLED = 1<<1
	}

	public enum NxDebugColor : uint
	{
		NX_ARGB_BLACK	= 0xff000000,
		NX_ARGB_RED		= 0xffff0000,
		NX_ARGB_GREEN	= 0xff00ff00,
		NX_ARGB_BLUE	= 0xff0000ff,
		NX_ARGB_YELLOW	= 0xffffff00,
		NX_ARGB_MAGENTA	= 0xffff00ff,
		NX_ARGB_CYAN	= 0xff00ffff,
		NX_ARGB_WHITE	= 0xffffffff,
	}

	public enum NxContactPairFlag
	{
		NX_IGNORE_PAIR				= (1<<0),	//!< Disable contact generation for this pair
		NX_NOTIFY_ON_START_TOUCH	= (1<<1),	//!< Pair callback will be called when the pair starts to be in contact
		NX_NOTIFY_ON_END_TOUCH		= (1<<2),	//!< Pair callback will be called when the pair stops to be in contact
		NX_NOTIFY_ON_TOUCH			= (1<<3),	//!< Pair callback will keep getting called while the pair is in contact
		NX_NOTIFY_ON_IMPACT			= (1<<4),	//!< [Not yet implemented] pair callback will be called when it may be appropriate for the pair to play an impact sound
		NX_NOTIFY_ON_ROLL			= (1<<5),	//!< [Not yet implemented] pair callback will be called when the pair is in contact and rolling.
		NX_NOTIFY_ON_SLIDE			= (1<<6),	//!< [Not yet implemented] pair callback will be called when the pair is in contact and sliding (and not rolling).
		NX_NOTIFY_ALL				= (NX_NOTIFY_ON_START_TOUCH|NX_NOTIFY_ON_END_TOUCH|NX_NOTIFY_ON_TOUCH|NX_NOTIFY_ON_IMPACT|NX_NOTIFY_ON_ROLL|NX_NOTIFY_ON_SLIDE)
	}
	
	public enum NxShapePairStreamFlags
	{
		NX_SF_HAS_MATS_PER_POINT		= (1<<0),	//!< used when we have materials per triangle in a mesh.  In this case the extData field is used after the point separation value.
		NX_SF_IS_INVALID				= (1<<1),	//!< this pair was invalidated in the system after being generated.  The user should ignore these pairs.
		NX_SF_HAS_FEATURES_PER_POINT	= (1<<2),	//!< the stream includes per-point feature data
		//note: bits 8-15 are reserved for internal use (static ccd pullback counter)
	}
	
	public enum NxMeshFlags
	{
		NX_MF_FLIPNORMALS		=	(1<<0),
		NX_MF_16_BIT_INDICES	=	(1<<1),	//<! Denotes the use of 16-bit vertex indices
		NX_MF_HARDWARE_MESH		=	(1<<2)	//<! The mesh will be used in hardware scenes
	}

	public enum NxInternalFormat
	{
		NX_FORMAT_NODATA,		//!< No data available
		NX_FORMAT_FLOAT,		//!< Data is in floating-point format
		NX_FORMAT_BYTE,			//!< Data is in byte format (8 bit)
		NX_FORMAT_SHORT,		//!< Data is in short format (16 bit)
		NX_FORMAT_INT			//!< Data is in int format (32 bit)
	}

	public enum NxInternalArray
	{
		NX_ARRAY_TRIANGLES,		//!< Array of triangles (index buffer). One triangle = 3 vertex references in returned format.
		NX_ARRAY_VERTICES,		//!< Array of vertices (vertex buffer). One vertex = 3 coordinates in returned format.
		NX_ARRAY_NORMALS,		//!< Array of vertex normals. One normal = 3 coordinates in returned format.
		NX_ARRAY_HULL_VERTICES,	//!< Array of hull vertices. One vertex = 3 coordinates in returned format.
		NX_ARRAY_HULL_POLYGONS	//!< Array of hull polygons
	}

	public enum NxMeshShapeFlag
	{
		NX_MESH_SMOOTH_SPHERE_COLLISIONS	= (1<<0),		
		NX_MESH_DOUBLE_SIDED				= (1<<1)	//!< The mesh is double-sided. This is currently only used for raycasting.
	}

	public enum NxFilterOp
	{
		NX_FILTEROP_AND,
		NX_FILTEROP_OR,
		NX_FILTEROP_XOR,
		NX_FILTEROP_NAND,
		NX_FILTEROP_NOR,
		NX_FILTEROP_NXOR,
		NX_FILTEROP_SWAP_AND
	}

	public enum NxConvexFlags
	{
		NX_CF_FLIPNORMALS		=	(1<<0),
		NX_CF_16_BIT_INDICES	=	(1<<1),	//<! Denotes the use of 16-bit vertex indices
		NX_CF_COMPUTE_CONVEX	=	(1<<2),	//<! Automatically recomputes the hull from the vertices
		NX_CF_INFLATE_CONVEX	=	(1<<3),
		NX_CF_USE_LEGACY_COOKER	=	(1<<4)	//!< Use the legacy convex hull algorithm.
	}

	public enum NxPlatform
	{
		PLATFORM_PC,
		PLATFORM_XENON,
		PLATFORM_PLAYSTATION3
	}

	public enum NxShapesType
	{
		NX_STATIC_SHAPES		= (1<<0),								//!< Hits static shapes
		NX_DYNAMIC_SHAPES		= (1<<1),								//!< Hits dynamic shapes
		NX_ALL_SHAPES			= NX_STATIC_SHAPES|NX_DYNAMIC_SHAPES	//!< Hits both static & dynamic shapes
	}

	public enum NxRaycastBit
	{
		NX_RAYCAST_SHAPE		= (1<<0),								//!< "shape" member of NxRaycastHit is valid
		NX_RAYCAST_IMPACT		= (1<<1),								//!< "worldImpact" member of NxRaycastHit is valid
		NX_RAYCAST_NORMAL		= (1<<2),								//!< "worldNormal" member of NxRaycastHit is valid
		NX_RAYCAST_FACE_INDEX	= (1<<3),								//!< "faceID" member of NxRaycastHit is valid
		NX_RAYCAST_DISTANCE		= (1<<4),								//!< "distance" member of NxRaycastHit is valid
		NX_RAYCAST_UV			= (1<<5),								//!< "u" and "v" members of NxRaycastHit are valid
		NX_RAYCAST_FACE_NORMAL	= (1<<6),								//!< Same as NX_RAYCAST_NORMAL but computes a non-smoothed normal
		NX_RAYCAST_MATERIAL		= (1<<7)								//!< "material" member of NxRaycastHit is valid
	}

	public enum NxQueryFlags
	{
		NX_QUERY_WORLD_SPACE	= (1<<0),	// world-space parameter, else object space
		NX_QUERY_FIRST_CONTACT	= (1<<1)	// returns first contact only, else returns all contacts
	}

	public enum NxTriangleFlags
	{
		// Must be the 3 first ones to be indexed by (flags & (1<<edge_index))
		NXTF_ACTIVE_EDGE01	= (1<<0),
		NXTF_ACTIVE_EDGE12	= (1<<1),
		NXTF_ACTIVE_EDGE20	= (1<<2),
		NXTF_DOUBLE_SIDED	= (1<<3),
		NXTF_BOUNDARY_EDGE01= (1<<4),
		NXTF_BOUNDARY_EDGE12= (1<<5),
		NXTF_BOUNDARY_EDGE20= (1<<6),
	}

	public enum NxBSphereMethod : uint
	{
		NX_BS_NONE,
		NX_BS_GEMS,
		NX_BS_MINIBALL
	}

	public enum NxFluidFlag
	{
		NX_FF_VISUALIZATION							= (1<<0),
		NX_FF_DISABLE_GRAVITY						= (1<<1)
	}
	
	public enum NxFluidParticleAction
	{
		NX_F_TURN_DEAD		= (1<<1),
		NX_F_TURN_SIMPLE	= (1<<2)
	}

	public enum NxFluidCollisionMethod
	{
		NX_F_STATIC					= (1<<0),	
		NX_F_DYNAMIC 				= (1<<1)
	}
	
	public enum NxFluidSimulationMethod
	{
		NX_F_SPH						= (1<<0),	
		NX_F_NO_PARTICLE_INTERACTION	= (1<<1),
		NX_F_MIXED_MODE					= (1<<2),
	}

	public enum NxParticleFlag
	{
		NX_FP_DIED						= (1<<0),	
		NX_FP_LIFETIME_EXPIRED			= (1<<1),
		NX_FP_DRAINED					= (1<<2),
		NX_FP_SEPARATED					= (1<<3)
	}

	public enum NxFluidEmitterFlag
	{
		NX_FEF_VISUALIZATION		= (1<<0),
		NX_FEF_BROKEN_ACTOR_REF		= (1<<1),
		NX_FEF_FORCE_ON_ACTOR		= (1<<2),
		NX_FEF_ADD_ACTOR_VELOCITY	= (1<<3),
		NX_FEF_ENABLED				= (1<<4)
	}

	public enum NxEmitterShape
	{
		NX_FE_RECTANGULAR		= (1<<0),
		NX_FE_ELLIPSE			= (1<<1)
	}
	
	public enum NxEmitterType
	{
		NX_FE_CONSTANT_PRESSURE		= (1<<0),
		NX_FE_CONSTANT_FLOW_RATE	= (1<<1)
	}

	public enum NxControllerType : uint
	{
		NX_CONTROLLER_BOX,
		NX_CONTROLLER_CAPSULE,
		UNKNOWN					//this is my value
	}

	public enum NxControllerFlag
	{
		NXCC_COLLISION_SIDES	= (1<<0),	//!< Character is colliding to the sides.
		NXCC_COLLISION_UP		= (1<<1),	//!< Character has collision above.
		NXCC_COLLISION_DOWN		= (1<<2)	//!< Character has collision below.
	}

	public enum NxControllerAction
	{
		NX_ACTION_NONE,				//!< Don't apply forces to touched actor
		NX_ACTION_PUSH				//!< Automatically compute & apply forces to touched actor (push)
	}

	public enum NxWheelShapeFlags
	{
		NX_WF_WHEEL_AXIS_CONTACT_NORMAL = 1 << 0,	//brief Determines whether the suspension axis or the ground contact normal is used for the suspension constraint.
		NX_WF_INPUT_LAT_SLIPVELOCITY = 1 << 1,		//brief If set, the laterial slip velocity is used as the input to the tire function, rather than the slip angle.
		NX_WF_INPUT_LNG_SLIPVELOCITY = 1 << 2,		//brief If set, the longutudal slip velocity is used as the input to the tire function, rather than the slip ratio.  
		NX_WF_UNSCALED_SPRING_BEHAVIOR = 1 << 3,	//brief If set, does not factor out the suspension travel and wheel radius from the spring force computation.  This is the legacy behavior from the raycast capsule approach.
		NX_WF_AXLE_SPEED_OVERRIDE = 1 << 4,			//brief If set, the axle speed is not computed by the simulation but is rather expected to be provided by the user every simulation step via NxWheelShape::setAxleSpeed().
	}

	public enum TriangleCollisionFlag
	{
		// Must be the 3 first ones to be indexed by (flags & (1<<edge_index))
		TCF_ACTIVE_EDGE01	= (1<<0),
		TCF_ACTIVE_EDGE12	= (1<<1),
		TCF_ACTIVE_EDGE20	= (1<<2),
		TCF_DOUBLE_SIDED	= (1<<3),
		//	TCF_WALKABLE		= (1<<4),
	}

	public enum TouchedGeomType : uint
	{
		TOUCHED_CONTROLLER,
		TOUCHED_MESH,
		TOUCHED_BOX,
		TOUCHED_SPHERE,
		TOUCHED_CAPSULE,
		TOUCHED_LAST
	}

	public enum SweptContactType
	{
		SWEPT_CTC_SHAPE,		// We touched another shape
		SWEPT_CTC_CONTROLLER,	// We touched another controller
	}
	
	public enum SweptVolumeType
	{
		SWEPT_BOX,
		SWEPT_SPHERE,
		SWEPT_ELLIPSOID,
		SWEPT_LAST
	}

	public enum NxHWVersion
	{
		NX_HW_VERSION_NONE = 0,
		NX_HW_VERSION_ATHENA_1_0 = 1
	}

	public enum NxMeshDataFlags
	{
		NX_MDF_16_BIT_INDICES				=	1 << 0,		//Denotes the use of 16-bit vertex indices.
		NX_MDF_16_BIT_COMPRESSED_FLOATS 	=	1 << 1,		//Specifies that all floats are written compressed to 16 bit.
		NX_MDF_INDEXED_MESH					=	1 << 2,		//Specifies that triangle indices are generated and adjacent triangles share common vertices. If this flag is not set, all triangles are described as vertex triplets in the vertex array.
	}

	public enum NxClothMeshTarget
	{
		NX_CLOTH_MESH_SOFTWARE		= 0,
		NX_CLOTH_MESH_PPU_ATHENA	= 1
	}

	public enum NxClothFlag
	{
		NX_CLF_PRESSURE			  = (1<<0),
		NX_CLF_STATIC			  = (1<<1),
		NX_CLF_DISABLE_COLLISION  = (1<<2),
		NX_CLF_SELFCOLLISION	  = (1<<3),
		NX_CLF_VISUALIZATION	  = (1<<4),
		NX_CLF_GRAVITY            = (1<<5),
		NX_CLF_BENDING            = (1<<6),
		NX_CLF_BENDING_ORTHO      = (1<<7),
		NX_CLF_DAMPING            = (1<<8),
		NX_CLF_COLLISION_TWOWAY   = (1<<9),
		NX_CLF_TRIANGLE_COLLISION = (1<<11),
		NX_CLF_TEARABLE           = (1<<12),
		NX_CLF_HARDWARE           = (1<<13)
	}

	public enum NxClothInteractionMode
	{
		NX_CLOTH_INTERACTION_ONEWAY = 0,	//only object->cloth interaction
		NX_CLOTH_INTERACTION_TWOWAY = 1		//both object->cloth and cloth->object interaction
	}

	public enum NxSDKCreationFlag
	{
		NX_SDKF_NO_HARDWARE							= (1<<0)
	}

	public enum NxCookingValue
	{
		NX_COOKING_CONVEX_VERSION_PC,
		NX_COOKING_MESH_VERSION_PC,
		NX_COOKING_CONVEX_VERSION_XENON,
		NX_COOKING_MESH_VERSION_XENON,
	}

	public enum NxSepAxis : uint
	{
		NX_SEP_AXIS_OVERLAP,
		NX_SEP_AXIS_A0,
		NX_SEP_AXIS_A1,
		NX_SEP_AXIS_A2,
		NX_SEP_AXIS_B0,
		NX_SEP_AXIS_B1,
		NX_SEP_AXIS_B2,
		NX_SEP_AXIS_A0_CROSS_B0,
		NX_SEP_AXIS_A0_CROSS_B1,
		NX_SEP_AXIS_A0_CROSS_B2,
		NX_SEP_AXIS_A1_CROSS_B0,
		NX_SEP_AXIS_A1_CROSS_B1,
		NX_SEP_AXIS_A1_CROSS_B2,
		NX_SEP_AXIS_A2_CROSS_B0,
		NX_SEP_AXIS_A2_CROSS_B1,
		NX_SEP_AXIS_A2_CROSS_B2
	}

	public enum NxHeightFieldAxis
	{		
		NX_X = 0,
		NX_Y = 1,
		NX_Z = 2,	
		NX_NOT_HEIGHTFIELD	= 0xff
	}

	public enum NxHeightFieldFormat
	{
		NX_HF_S16_TM = (1 << 0)
		//Height field height data is 16 bit signed integers, followed by triangle materials. 
		//Each sample is 32 bits wide arranged as follows:
		//1) First there is a 16 bit height value.
		//2) Next, two one byte material indices, with the high bit of each byte reserved for special use.
		//(so the material index is only 7 bits).
		//The high bit of material0 is the tess-flag.
		//The high bit of material1 is reserved for future use.
	}

	public enum NxHeightFieldTessFlag
	{
		NX_HF_0TH_VERTEX_SHARED = (1 << 0)
	}

	public enum NxHeightFieldFlags
	{
		NX_HF_NO_BOUNDARY_EDGES = (1 << 0)
	}

	public enum NxThreadPollResult	: uint
	{
		NX_THREAD_NOWORK				= 0,
		NX_THREAD_MOREWORK				= 1,
		NX_THREAD_SIMULATION_END		= 2,
		NX_THREAD_SHUTDOWN				= 3
	}

	public enum NxThreadWait : uint
	{
		NX_WAIT_NONE				= 0,
		NX_WAIT_SIMULATION_END		= 1,
		NX_WAIT_SHUTDOWN			= 2
	}

	public enum NxInterfaceType
	{
		NX_INTERFACE_STATS,
		NX_INTERFACE_LAST
	}

	public enum NxRemoteDebuggerObjectType
	{
		NX_DBG_OBJECTTYPE_GENERIC = 0,
		NX_DBG_OBJECTTYPE_ACTOR = 1,
		NX_DBG_OBJECTTYPE_PLANE = 2,
		NX_DBG_OBJECTTYPE_BOX = 3,
		NX_DBG_OBJECTTYPE_SPHERE = 4,
		NX_DBG_OBJECTTYPE_CAPSULE = 5,
		NX_DBG_OBJECTTYPE_CYLINDER = 6,
		NX_DBG_OBJECTTYPE_CONVEX = 7,
		NX_DBG_OBJECTTYPE_MESH = 8,
		NX_DBG_OBJECTTYPE_WHEEL = 9,
		NX_DBG_OBJECTTYPE_JOINT = 10,
		NX_DBG_OBJECTTYPE_CONTACT = 11,
		NX_DBG_OBJECTTYPE_BOUNDINGBOX = 12,
		NX_DBG_OBJECTTYPE_VECTOR = 13,
		NX_DBG_OBJECTTYPE_CAMERA = 14
	}
}

