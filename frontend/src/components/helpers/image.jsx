import React, {useEffect, useState} from 'react';

const Image = ({ imageClassName, containerClassName, src, defaultImage, alt }) => {
  const [imageSrc, setImageSrc] = useState(defaultImage?.toString())

  useEffect(()=>{
    setImageSrc(src)
  }, [src])

  const handleError = (event) => {
    event.currentTarget.onerror = null;
    setImageSrc(defaultImage);
  };

  return (
    <div className={containerClassName}>
      <img src={imageSrc ? imageSrc : defaultImage}
           className={imageClassName}
           alt= { alt ? alt : "img" }
           onError={(event) => handleError(event)} />
    </div>
  );
};

export default Image