<?php

    namespace infra\repositories;    
    use infra\MySqlRepository;
    use infra\interfaces\IStateRepository;
    use models\State;
    use PDO;

    
    class StateRepository 
        extends MySqlRepository 
        implements IStateRepository
    {
       
        public function getAll()
        {
            $stmt = $this->conn->prepare( 
                "SELECT stateId, name, stateAbreviattion 
                FROM States order by Name" 
            );
            $stmt->execute();
            $usersResults = $stmt->fetchAll();
           
            $states = array();

            foreach($usersResults as $state){
                $states[] = new State(
                    $state["stateId"],
                    $state["name"],
                    $state["stateAbreviattion"]
                );
            }
            return $states;
        }
    }

?>