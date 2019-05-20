


(defclass Node [object]
  (defn --init-- [self name]
    (setv self.name name)
    (setv self.succ [])
    (setv self.prev [])
    (setv self.use [])
    (setv self.def [])))



(defn main []
  (let [n0 (Node "1")]
       (print n0)))

(if (= --name-- "__main__")
  (main))

